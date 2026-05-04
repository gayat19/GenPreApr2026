using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnderstandingOOPSApp.Models
{
    public enum AccType
    {
        SavingAccount =1,CurrentAccount=2
    }
    internal class Account 
    {
        

        public  string AccountNumber { get; set; } =string.Empty;
        public string NameOnAccount { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public float Balance { get; set; }
        public AccType AccountType { get; set; }
        public Account()
        {
            
        }

        public Account(string accountNumber, string nameOnAccount, DateTime dateOfBirth, string email, string phone, float balance)
        {
            AccountNumber = accountNumber;
            NameOnAccount = nameOnAccount;
            DateOfBirth = dateOfBirth;
            Email = email;
            Phone = phone;
            Balance = balance;
        }
        public override string ToString()
        {
            return $"Account Number : {AccountNumber}\nAccountType : {AccountType}\nAccount Holder Name : {NameOnAccount}\nPhone Number : {Phone}\n" +
                $"Email : {Email}\nBalance : ${Balance}";
        }
    }
}
