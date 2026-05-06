using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingModelLibrary
{
    public class SavingAccount :Account
    {
        public SavingAccount()
        {
            AccountType = AccType.SavingAccount;
            Balance = 100.0f;
        }
    }
}
