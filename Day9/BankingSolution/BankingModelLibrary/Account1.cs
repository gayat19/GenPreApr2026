using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingModelLibrary
{
    public partial class Account : IComparable<Account>, IEquatable<Account>
    {
       
        public override string ToString()
        {
            return $"Account Number : {AccountNumber}\nAccountType : {AccountType}\nAccount Holder Name : {NameOnAccount}\nPhone Number : {Phone}\n" +
                $"Email : {Email}\nBalance : ${Balance}";
        }

        public int CompareTo(Account? other)
        {
            return this.AccountNumber.CompareTo(other.AccountNumber);
        }

        public bool Equals(Account? other)
        {
            return (this.AccountNumber == other.AccountNumber);
        }
    }
}
