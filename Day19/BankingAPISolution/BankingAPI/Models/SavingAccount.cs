using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAPI.Models
{
    public class SavingAccount :Account
    {
        public SavingAccount()
        {
            Balance = 100.0f;
        }
    }
}
