using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnderstandingOOPSApp.Interfaces;
using UnderstandingOOPSApp.Models;

namespace UnderstandingOOPSApp.Services
{
    internal class CustomerService : ICustomerInteract
    {
        List<Account> accounts = new List<Account>();
        static string lastAccountNumber = "9990001000";
        public Account OpensAccount()
        {
            Account account = TakeCustomerDetails();
            TakeInitialDeposit(account);
            long accNum = Convert.ToInt64(lastAccountNumber);
            account.AccountNumber =  (++accNum).ToString();
            lastAccountNumber = accNum.ToString();
            accounts.Add(account);
            return account;
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
            Account account = null;
            foreach (var item in accounts)
            {
                if(item.AccountNumber == accountNumber)
                {
                    account = item;
                    break;
                }
            }
            if (account != null)
            {
                PrintAccount(account);
                return;
            }
            Console.WriteLine("No account with the given number is present with us");
            
        }

        private void PrintAccount(Account account)
        {
            Console.WriteLine("-----------------------------");
            Console.WriteLine(account);
            Console.WriteLine("-----------------------------");
        }
    }
}
