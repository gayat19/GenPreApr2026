using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnderstandingOOPSApp.Models
{
    internal class SavingAccount :Account
    {
        public SavingAccount()
        {
            AccountType = AccType.SavingAccount;
            Balance = 100.0f;
        }
    }
}
