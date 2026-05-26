using BankingAPI.Contexts;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Models.DTOs;
using BankingAPI.Repositories;
using BankingAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApiTest
{
    public class CustomerServiceTest
    {
        
        ICustomerInteract customerInteract;
        IRepository<int, Customer> customerRepository;
        IRepository<string, Account> accountRepository;
        IRepository<string, User> userRepository;
       [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BankingContext>().UseInMemoryDatabase("BankingDb").Options;
            BankingContext bankingContext = new BankingContext(options);
            customerRepository = new Repository<int, Customer>(bankingContext);
            accountRepository = new Repository<string, Account>(bankingContext);
            userRepository = new Repository<string, User>(bankingContext);
            string configJson = """
                {
                  { "Key","ThisismySuperSecretKeyOn22052026" },
                  { "Issuer","BankingAPI" },
                  { "ExpiryInMinutes", "60" }
                }
                """;
            var inMemorySettings = new Dictionary<string, string> {
                   { "JWT",configJson}
            };
            IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            ITokenService tokenService = new TokenService(configuration);

            customerInteract = new CustomerService(accountRepository,userRepository, customerRepository,tokenService);
        }


        [Test]
        public async Task OpenAccountPassTest()
        {
            //Arrange
            Customer customer = new Customer
            {
                Id = 1,
                Name = "John Doe",
                Email = "john@test.com",
                DateOfBirth = new DateTime(2000, 1, 1),
                Phone = "1234567890",
                Status = "Active",
                Username = null
            };
            customer = await customerRepository.Create(customer);
            CreateAccountRequest request = new CreateAccountRequest
            {
                CustomerId = customer.Id,
                Balance = 1000,
                AccountType = "Account"
            };  
            var result = await customerInteract.OpensAccount(request);
            Assert.That(result.AccountNumber, Is.Not.Null);
        }

        [TearDown]
        public void TearDown()
        {
            var options = new DbContextOptionsBuilder<BankingContext>().UseInMemoryDatabase("BankingDb").Options;
            BankingContext bankingContext = new BankingContext(options);
            bankingContext.Database.EnsureDeleted();
        }

    }
}
