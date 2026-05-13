using BankingBLLibrary.Interfaces;
using BankingModelLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BankingDALLibrary.Interfaces;
using BankingDALLibrary.Repositories;
using BankingModelLibrary.Exceptions;


namespace BankingBLLibrary.Services
{
    public class CustomerService : ICustomerInteract
    {
        IRepository<string, Account> accountRespository;

        public CustomerService()
        {
            accountRespository = new AccountRepository();
        }
        public Account OpensAccount()
        {
            try
            {
               
                var account = TakeCustomerDetails();
                Regex regex = new Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$\r\n");
                //if (!regex.IsMatch(account.Email))
                //    throw new InvalidContactDetailException("Invalid Email");
                account = accountRespository.Create(account);
                return account;
            }
            catch (FormatException fe)
            {
                Console.WriteLine("The input for teh account details was not in proper format");
            }
            catch (OverflowException ofe)
            {
                Console.WriteLine("Unable to genereate account n ow");
                Console.WriteLine(ofe.Message);
            }
            catch (InvalidContactDetailException ipne)
            {
                Console.WriteLine("Unable to create account since the contact details(Email or phone) you entered seems to be invalid");
                Console.WriteLine(ipne.Message);
            }
            finally
            {
                accountRespository = null;
                Console.WriteLine("Repository released");
            }
            return null;
        }

        private void TakeInitialDeposit(Account account)
        {
            Console.WriteLine("Do you want to deposit any amount now. If yes enter the amount. else enter 0.?");
            float amount = 0;
            while(!float.TryParse(Console.ReadLine(), out amount))
                Console.WriteLine("Invalid entry. Please enter the deposit amount");
            account.Balance += amount;

        }

        private Account TakeCustomerDetails()
        {
            Account account = new Account();
            Console.WriteLine("Please select the type of account you want to open. 1 for savings. 2 for current");
            int typeChoice;
            while(!int.TryParse(Console.ReadLine(), out typeChoice) && typeChoice>0 && typeChoice<3)
                Console.WriteLine("Invalid entry. Please try again");
            if(typeChoice == 1)
                account = new SavingAccount();
            if(typeChoice == 2)
                account = new CurrentAccount();
           
           
            return account;

        }

        public void PrintAccountDetails(string accountNumber)
        {
            var account = accountRespository.Get(accountNumber);
            if (account == null)
                throw new Exception("NO account found");
            PrintAccount(account);
            
        }

        private void PrintAccount(Account account)
        {
            Console.WriteLine("-----------------------------");
            Console.WriteLine(account);
            Console.WriteLine("-----------------------------");
        }
    }
}
