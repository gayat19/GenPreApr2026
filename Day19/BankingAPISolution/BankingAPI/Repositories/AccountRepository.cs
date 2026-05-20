using BankingAPI.Contexts;
using BankingAPI.Models;
using BankingAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;


namespace BankingDALLibrary.Repositories
{
    public class AccountRepository : AbstractRepository<string, Account>
    {
        public AccountRepository(BankingContext context) : base(context)
        {
            
        }
        public override Account? Get(string key)
        {
            var account = _context.Accounts
                .Include(a=>a.Customer)//includes teh custoemr data while loading teh account data
                .SingleOrDefault(a => a.AccountNumber == key);
            return account;
        }
    }
}
