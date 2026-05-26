using BankingAPI.Models.DTOs;
using System.Collections.Generic;

namespace BankingAPI.Interfaces
{
    public interface ITransact
    {
        Task<TransactionResponse> Deposit(DepositRequest request);
        Task<TransactionResponse> Withdraw(WithdrawRequest request);
        Task<TransactionResponse> Transfer(TransferRequest request);

        // Replaced simple account query with a flexible query API
        Task<TransactionQueryResponse> QueryTransactions(TransactionQueryRequest request);
        Task<TransactionResponse?> GetTransactionByReference(int referenceNumber);
    }
}
