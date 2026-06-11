using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Models.DTOs;
using BankingAPI.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingAPI.Services
{
    public class TransactionService : ITransact
    {
        private readonly BankingContext _context;

        public TransactionService(BankingContext context)
        {
            _context = context;
        }

        public async Task<TransactionResponse> Deposit(DepositRequest request)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(a => a.AccountNumber == request.ToAccountNumber);
            if (account == null)
                throw new ArgumentException("Destination account not found: " + request.ToAccountNumber);

            await using var dbTxn = await _context.Database.BeginTransactionAsync();
            try
            {
                // update balance
                account.Balance += (float)request.Amount;
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();

                // create transaction record
                var tx = new Transaction
                {
                    TransactionDate = DateTime.UtcNow,
                    FromAccountNumber = string.Empty,
                    ToAccountNumber = account.AccountNumber,
                    Amount = request.Amount,
                    Status = "Success"
                };

                var created = _context.Transactions.Add(tx);
                await _context.SaveChangesAsync();

                await dbTxn.CommitAsync();

                return Map(created.Entity);
            }
            catch
            {
                await dbTxn.RollbackAsync();
                throw;
            }
        }

        public async Task<TransactionResponse> Withdraw(WithdrawRequest request)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(a => a.AccountNumber == request.FromAccountNumber);
            if (account == null)
                throw new ArgumentException("Source account not found: " + request.FromAccountNumber);

            var amt = (float)request.Amount;
            if (account.Balance < amt)
                throw new InvalidOperationException("Insufficient funds");

            await using var dbTxn = await _context.Database.BeginTransactionAsync();
            try
            {
                account.Balance -= amt;
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();

                var tx = new Transaction
                {
                    TransactionDate = DateTime.UtcNow,
                    FromAccountNumber = account.AccountNumber,
                    ToAccountNumber = string.Empty,
                    Amount = request.Amount,
                    Status = "Success"
                };

                var created = _context.Transactions.Add(tx);
                await _context.SaveChangesAsync();

                await dbTxn.CommitAsync();

                return Map(created.Entity);
            }
            catch
            {
                await dbTxn.RollbackAsync();
                throw;
            }
        }

        public async Task<TransactionResponse> Transfer(TransferRequest request)
        {
            if (request.FromAccountNumber == request.ToAccountNumber)
                throw new ArgumentException("From and To account numbers must differ.");

            var from = await _context.Accounts.SingleOrDefaultAsync(a => a.AccountNumber == request.FromAccountNumber);
            var to = await _context.Accounts.SingleOrDefaultAsync(a => a.AccountNumber == request.ToAccountNumber);

            if (from == null)
                throw new ArgumentException("Source account not found: " + request.FromAccountNumber);
            if (to == null)
                throw new ArgumentException("Destination account not found: " + request.ToAccountNumber);

            var amt = (float)request.Amount;
            if (from.Balance < amt)
                throw new InvalidOperationException("Insufficient funds in source account");

            await using var dbTxn = await _context.Database.BeginTransactionAsync();
            try
            {
                from.Balance -= amt;
                to.Balance += amt;

                _context.Accounts.Update(from);
                _context.Accounts.Update(to);
                await _context.SaveChangesAsync();

                // If after transfer source balance is below 1000, rollback entire transaction
                if (from.Balance < 1000f)
                {
                    var failedTx = new Transaction
                    {
                        TransactionDate = DateTime.UtcNow,
                        FromAccountNumber = from.AccountNumber,
                        ToAccountNumber = to.AccountNumber,
                        Amount = request.Amount,
                        Status = "Failed - Post-transfer balance below minimum"
                    };

                    _context.Transactions.Add(failedTx);
                    await _context.SaveChangesAsync();

                    await dbTxn.RollbackAsync();
                    throw new InvalidOperationException("Transfer would reduce source balance below required minimum of 1000. Transaction rolled back.");
                }

                var tx = new Transaction
                {
                    TransactionDate = DateTime.UtcNow,
                    FromAccountNumber = from.AccountNumber,
                    ToAccountNumber = to.AccountNumber,
                    Amount = request.Amount,
                    Status = "Success"
                };

                var created = _context.Transactions.Add(tx);
                await _context.SaveChangesAsync();

                await dbTxn.CommitAsync();

                return Map(created.Entity);
            }
            catch
            {
                try { await dbTxn.RollbackAsync(); } catch { /* ignore rollback errors */ }
                throw;
            }
        }

        public async Task<TransactionQueryResponse> QueryTransactions(TransactionQueryRequest request)
        {
            var q = _context.Transactions.AsQueryable();

            // filters
            if (!string.IsNullOrWhiteSpace(request.FromAccountNumber))
            {
                q = q.Where(t => t.FromAccountNumber == request.FromAccountNumber);
            }

            if (request.MinAmount.HasValue)
            {
                q = q.Where(t => t.Amount >= request.MinAmount.Value);
            }

            if (request.MaxAmount.HasValue)
            {
                q = q.Where(t => t.Amount <= request.MaxAmount.Value);
            }

            if (request.FromDate.HasValue)
            {
                q = q.Where(t => t.TransactionDate >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                q = q.Where(t => t.TransactionDate <= request.ToDate.Value);
            }

            // sorting
            var sortBy = (request.SortBy ?? "TransactionDate").Trim().ToLowerInvariant();
            var asc = string.Equals(request.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);

            q = sortBy switch
            {
                "amount" => asc ? q.OrderBy(t => t.Amount) : q.OrderByDescending(t => t.Amount),
                "fromaccountnumber" => asc ? q.OrderBy(t => t.FromAccountNumber) : q.OrderByDescending(t => t.FromAccountNumber),
                "toaccountnumber" => asc ? q.OrderBy(t => t.ToAccountNumber) : q.OrderByDescending(t => t.ToAccountNumber),
                _ => asc ? q.OrderBy(t => t.TransactionDate) : q.OrderByDescending(t => t.TransactionDate)
            };

            // pagination
            var totalCount = await q.CountAsync();
            var pageSize = Math.Max(1, request.PageSize);
            var pageNumber = Math.Max(1, request.PageNumber);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var itemsEntities = await q
                        .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            var items = itemsEntities.Select(Map).ToList();

            return new TransactionQueryResponse
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<TransactionResponse?> GetTransactionByReference(int referenceNumber)
        {
            var tx = await _context.Transactions.FindAsync(referenceNumber);
            if (tx == null) return null;
            return Map(tx);
        }

        private TransactionResponse Map(Transaction t)
        {
            return new TransactionResponse
            {
                TransactionReferenceNumber = t.TransactionReferenceNumber,
                TransactionDate = t.TransactionDate,
                FromAccountNumber = t.FromAccountNumber ?? string.Empty,
                ToAccountNumber = t.ToAccountNumber ?? string.Empty,
                Amount = t.Amount,
                Status = t.Status ?? string.Empty
            };
        }
    }
}
