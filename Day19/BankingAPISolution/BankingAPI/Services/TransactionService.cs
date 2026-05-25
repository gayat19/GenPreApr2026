using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Models.DTOs;
using BankingAPI.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankingAPI.Services
{
    public class TransactionService : ITransact
    {
        private readonly BankingContext _context;

        public TransactionService(BankingContext context)
        {
            _context = context;
        }

        public TransactionResponse Deposit(DepositRequest request)
        {
            var account = _context.Accounts.SingleOrDefault(a => a.AccountNumber == request.ToAccountNumber);
            if (account == null)
                throw new ArgumentException("Destination account not found: " + request.ToAccountNumber);

            using var dbTxn = _context.Database.BeginTransaction();
            try
            {
                // update balance
                account.Balance += (float)request.Amount;
                _context.Accounts.Update(account);
                _context.SaveChanges();

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
                _context.SaveChanges();

                dbTxn.Commit();

                return Map(created.Entity);
            }
            catch
            {
                dbTxn.Rollback();
                throw;
            }
        }

        public TransactionResponse Withdraw(WithdrawRequest request)
        {
            var account = _context.Accounts.SingleOrDefault(a => a.AccountNumber == request.FromAccountNumber);
            if (account == null)
                throw new ArgumentException("Source account not found: " + request.FromAccountNumber);

            var amt = (float)request.Amount;
            if (account.Balance < amt)
                throw new InvalidOperationException("Insufficient funds");

            using var dbTxn = _context.Database.BeginTransaction();
            try
            {
                account.Balance -= amt;
                _context.Accounts.Update(account);
                _context.SaveChanges();

                var tx = new Transaction
                {
                    TransactionDate = DateTime.UtcNow,
                    FromAccountNumber = account.AccountNumber,
                    ToAccountNumber = string.Empty,
                    Amount = request.Amount,
                    Status = "Success"
                };

                var created = _context.Transactions.Add(tx);
                _context.SaveChanges();

                dbTxn.Commit();

                return Map(created.Entity);
            }
            catch
            {
                dbTxn.Rollback();
                throw;
            }
        }

        public TransactionResponse Transfer(TransferRequest request)
        {
            if (request.FromAccountNumber == request.ToAccountNumber)
                throw new ArgumentException("From and To account numbers must differ.");

            var from = _context.Accounts.SingleOrDefault(a => a.AccountNumber == request.FromAccountNumber);
            var to = _context.Accounts.SingleOrDefault(a => a.AccountNumber == request.ToAccountNumber);

            if (from == null)
                throw new ArgumentException("Source account not found: " + request.FromAccountNumber);
            if (to == null)
                throw new ArgumentException("Destination account not found: " + request.ToAccountNumber);

            var amt = (float)request.Amount;
            if (from.Balance < amt)
                throw new InvalidOperationException("Insufficient funds in source account");

            using var dbTxn = _context.Database.BeginTransaction();
            try
            {
                from.Balance -= amt;
                to.Balance += amt;

                _context.Accounts.Update(from);
                _context.Accounts.Update(to);
                _context.SaveChanges();

                // If after transfer source balance is below 1000, rollback entire transaction
                if (from.Balance < 1000f)
                {
                    // create a failed transaction record before rollback (optional) or simply rollback
                    var failedTx = new Transaction
                    {
                        TransactionDate = DateTime.UtcNow,
                        FromAccountNumber = from.AccountNumber,
                        ToAccountNumber = to.AccountNumber,
                        Amount = request.Amount,
                        Status = "Failed - Post-transfer balance below minimum"
                    };

                    _context.Transactions.Add(failedTx);
                    _context.SaveChanges();

                    dbTxn.Rollback();
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
                _context.SaveChanges();

                dbTxn.Commit();

                return Map(created.Entity);
            }
            catch
            {
                // ensure rollback on any exception
                try { dbTxn.Rollback(); } catch { /* ignore errors on rollback */ }
                throw;
            }
        }

        public IEnumerable<TransactionResponse> GetTransactionsForAccount(string accountNumber)
        {
            var all = _context.Transactions
                .Where(t => t.FromAccountNumber == accountNumber || t.ToAccountNumber == accountNumber)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();

            return all.Select(Map).ToList();
        }

        public TransactionResponse? GetTransactionByReference(int referenceNumber)
        {
            var tx = _context.Transactions.Find(referenceNumber);
            if (tx == null) return null;
            return Map(tx);
        }

        private TransactionResponse Map(Transaction t)
        {
            return new TransactionResponse
            {
                TransactionReferenceNumber = t.TransactionReferenceNumber,
                TransactionDate = t.TransactionDate,
                FromAccountNumber = t.FromAccountNumber,
                ToAccountNumber = t.ToAccountNumber,
                Amount = t.Amount,
                Status = t.Status
            };
        }
    }
}
