using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Misc;
using BankingAPI.Models.DTOs;
using System.Security.Cryptography;
using System.Security.Authentication;



namespace BankingAPI.Services
{
    public class CustomerService : ICustomerInteract, IAuthenticationService
    {
        readonly IRepository<string, Account> _accountRespository;
        private readonly IRepository<string, User> _userRepository;
        private readonly IRepository<int, Customer> _customerRepository;
        private readonly ITokenService _tokenService;

        public CustomerService(IRepository<string,Account> accountRepository,
                                IRepository<string, User> userRepository,
                                IRepository<int, Customer> customerRepository,
                                ITokenService tokenService)
        {
            _accountRespository = accountRepository;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var dbUser = await _userRepository.Get(request.Username);
            if (dbUser == null)
                throw new InvalidCredentialException("Invalid username or password");
            HMACSHA256 hMACSHA256 = new HMACSHA256(dbUser.HashKey);
            var userHashPassword = hMACSHA256.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
            for (int i = 0; i < userHashPassword.Length; i++)
                if (userHashPassword[i] != dbUser.Password[i])
                    throw new InvalidCredentialException("Invalid username or password");
            var loginResponse = new LoginResponse();
            loginResponse.Username = request.Username;
            string givenName = (await _customerRepository.GetAll()).Where(c => c.Username == request.Username).ToList()[0].Name;
            loginResponse.Token = _tokenService.CreateNewToken(new TokenRequest
            {
                Username = request.Username,
                Role = dbUser.Role,
                GivenName = givenName
            });
            return loginResponse;

        }

        public async Task<CreateAccountResponse> OpensAccount(CreateAccountRequest account)
        {
            string newAccountNumber = await GenerateAccountNumber();
            Account newAccount = new Account()
            {
                AccountNumber = newAccountNumber,
                CustomerId = account.CustomerId,
                Balance = account.Balance,
                AccountType = account.AccountType,
                Status = "Active"
            };
            var result = await _accountRespository.Create(newAccount);
            return new CreateAccountResponse
            {
                AccountNumber = result.AccountNumber,
                Status = result.Status,
                Balance = result.Balance,
                AccountType = result.AccountType
            };
        }

        public async Task<RegisterUserResponse> Register(RegisterUserRequest request)
        {
            User user = await MapUserObjectFromRequest(request);
            Customer customer = await MapCustomerObjectFromRequest(request);
            user = await _userRepository.Create(user);
            customer.Username = user.Username;
            customer = await _customerRepository.Create(customer);
            if (user != null && customer != null)
                return new RegisterUserResponse
                {
                    CustomerId = customer.Id,
                };
            throw new UnableToCreateEntityException("User or customer object not created");

        }

        private async Task<Customer> MapCustomerObjectFromRequest(RegisterUserRequest request)
        {
            return  new Customer
            {
                Email = request.Email,
                Phone = request.Phone,
                Name = request.Name,
                DateOfBirth = request.DateOfBirth,
                Status = "Active"
            };
        }

        private async Task<User> MapUserObjectFromRequest(RegisterUserRequest request)
        {
            HMACSHA256 hMACSHA256 = new HMACSHA256();
            User user = new User();
            user.Username = request.Username;
            user.Password = hMACSHA256.ComputeHash(Encoding.UTF8.GetBytes(user.Username.Substring(0,4)+"1234"));
            user.HashKey = hMACSHA256.Key;
            user.Role = "Customer";
            return user;
        }



        private async Task<string> GenerateAccountNumber()
        {
            string newAccountNumber = "";
            var accounts = (await _accountRespository.GetAll()).Count;
            if(accounts == 0)
                return "0009990001";
            var maxAccountNumber = (await _accountRespository.GetAll()).OrderByDescending(a => a.AccountNumber).ToList()[0].AccountNumber;
            var number = Convert.ToInt64(maxAccountNumber);
            number++;
            if(number> 999999999)
                newAccountNumber = "00"+number.ToString();
            else if(number> 99999999 && number< 999999998)
                newAccountNumber = "000" + number.ToString();
            return newAccountNumber;
        }

        async Task<GetAccountResponse> ICustomerInteract.GetAccountByAccountNumber(string accountNumber)
        {
            var account = await _accountRespository.Get(accountNumber);
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
