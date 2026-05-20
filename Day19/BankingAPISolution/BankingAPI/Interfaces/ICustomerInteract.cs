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
        public CreateAccountResponse OpensAccount(CreateAccountRequest account);
        public GetAccountResponse GetAccountByAccountNumber(string accountNumber);

    }
}
