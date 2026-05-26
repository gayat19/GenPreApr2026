using BankingAPI.Models;
using BankingAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BankingAPI.Interfaces
{
    public interface ICustomerInteract
    {
        public Task<CreateAccountResponse> OpensAccount(CreateAccountRequest account);
        public Task<GetAccountResponse> GetAccountByAccountNumber(string accountNumber);

    }
}
