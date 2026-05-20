using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAPI.Models
{
    public partial class Account : IComparable<Account>, IEquatable<Account>
    {
       
        public override string ToString()
        {
            return $"Account Number : {AccountNumber}\nAccountType : {AccountType}\nBalance : ${Balance}\nCustomer Details as follows\n{Customer}";
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
