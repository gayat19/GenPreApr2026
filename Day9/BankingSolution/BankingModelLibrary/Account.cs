using BankingModelLibrary.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingModelLibrary
{

    public partial class Account 
    {
        
        public  string AccountNumber { get; set; } =string.Empty;


        public float Balance { get; set; }
        public string AccountType { get; set; }

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public string Status { get; set; }
        public Account()
        {
            
        }

        public Account(string accountNumber, string nameOnAccount, DateTime dateOfBirth, string email, string phone, float balance)
        {
            AccountNumber = accountNumber;
            Balance = balance;
        }
       
    }
}
