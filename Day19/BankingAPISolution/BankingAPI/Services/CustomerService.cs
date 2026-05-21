using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Misc;
using BankingAPI.Models.DTOs;



namespace BankingAPI.Services
{
    public class CustomerService : ICustomerInteract, IAuthenticationService
    {
        readonly IRepository<string, Account> _accountRespository;
        private readonly IRepository<string, User> _userRepository;
        private readonly IRepository<int, Customer> _customerRepository;

        public CustomerService(IRepository<string,Account> accountRepository,
                                IRepository<string, User> userRepository,
                                IRepository<int, Customer> customerRepository)
        {
            _accountRespository = accountRepository;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
        }

        public LoginResponse Login(LoginRequest request)
        {
            throw new NotImplementedException();
        }

        public CreateAccountResponse OpensAccount(CreateAccountRequest account)
        {
            string newAccountNumber = GenerateAccountNumber();
            Account newAccount = new Account()
            {
                AccountNumber = newAccountNumber,
                CustomerId = account.CustomerId,
                Balance = account.Balance,
                AccountType = account.AccountType,
                Status = "Active"
            };
            var result = _accountRespository.Create(newAccount);
            return new CreateAccountResponse
            {
                AccountNumber = result.AccountNumber,
                Status = result.Status,
                Balance = result.Balance,
                AccountType = result.AccountType
            };
        }

        public RegisterUserResponse Register(RegisterUserRequest request)
        {
            throw new NotImplementedException();
        }

        private string GenerateAccountNumber()
        {
            string newAccountNumber = "";
            var maxAccountNumber = _accountRespository.GetAll().OrderByDescending(a => a.AccountNumber).ToList()[0].AccountNumber;
            var number = Convert.ToInt64(maxAccountNumber);
            number++;
            if(number> 999999999)
                newAccountNumber = "00"+number.ToString();
            else if(number> 99999999 && number< 999999998)
                newAccountNumber = "000" + number.ToString();
            return newAccountNumber;
        }

        GetAccountResponse ICustomerInteract.GetAccountByAccountNumber(string accountNumber)
        {
            var account = _accountRespository.Get(accountNumber);
            return new GetAccountResponse
            {
                AccountNumber = accountNumber,
                AccountType = account.AccountType,
                Balance = account.Balance,
                CustomerId = account.CustomerId,
                Status = account.Status
            };
        }
    }
}
