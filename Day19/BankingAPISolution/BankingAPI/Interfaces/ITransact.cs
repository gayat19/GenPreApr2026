using BankingAPI.Models.DTOs;
using System.Collections.Generic;

namespace BankingAPI.Interfaces
{
    public interface ITransact
    {
        TransactionResponse Deposit(DepositRequest request);
        TransactionResponse Withdraw(WithdrawRequest request);
        TransactionResponse Transfer(TransferRequest request);

        IEnumerable<TransactionResponse> GetTransactionsForAccount(string accountNumber);
        TransactionResponse? GetTransactionByReference(int referenceNumber);
    }
}
