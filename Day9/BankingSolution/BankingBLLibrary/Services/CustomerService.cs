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

        public Account OpensAccount()
        {
            try
            {
                accountRespository = new AccountRepository();
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
            Console.WriteLine("Please enter your full name");
            account.NameOnAccount = Console.ReadLine()??"";
            Console.WriteLine("Please enter your Date of birth in yyyy-mm--dd format");
            DateTime dob;
            while(!DateTime.TryParse(Console.ReadLine(),out dob ) && dob>DateTime.Today)
                Console.WriteLine("Invalid entry for date. Please try again");
            Console.WriteLine("Please enter your email address");
            account.Email = Console.ReadLine() ?? "";
            Console.WriteLine("Please enter your phone number");
            account.Phone = Console.ReadLine() ?? "";
            return account;

        }

        public void PrintAccountDetails(string accountNumber)
        {
            //Account account = null;
            //foreach (var item in accounts)
            //{
            //    if(item.AccountNumber == accountNumber)
            //    {
            //        account = item;
            //        break;
            //    }
            //}
            //if (account != null)
            //{
            //    PrintAccount(account);
            //    return;
            //}
            //Console.WriteLine("No account with the given number is present with us");
            
        }

        private void PrintAccount(Account account)
        {
            Console.WriteLine("-----------------------------");
            Console.WriteLine(account);
            Console.WriteLine("-----------------------------");
        }
    }
}
