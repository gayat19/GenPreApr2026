using BankingModelLibrary;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using BankingModelLibrary.Exceptions;
using Microsoft.EntityFrameworkCore;


namespace BankingDALLibrary.Repositories
{
    public class AccountRepository : AbstractRepository<string, Account>
    {
        public override Account? Get(string key)
        {
            var account = context.Accounts
                .Include(a=>a.Customer)//includes teh custoemr data while loading teh account data
                .SingleOrDefault(a => a.AccountNumber == key);
            return account;
        }
    }
}
