using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using UnderstandingOOPSApp.Interfaces;
using UnderstandingOOPSApp.Models;

namespace UnderstandingOOPSApp.Repositories
{
    internal class AccountRepository : AbstractRepository<string,Account>
    {
        public AccountRepository()
        {
            _items = new Dictionary<string, Account>();
        }

        public Account this[string index]
        {
            get { return _items[index]; }
            set { _items[index] = value; }
        }
        static string lastAccountNumber = "9990001000";
        public override Account Create(Account item)
        {
            long accNum = Convert.ToInt64(lastAccountNumber);
            item.AccountNumber = (++accNum).ToString();
            lastAccountNumber = accNum.ToString();
            _items.Add(lastAccountNumber, item);
            return item;
        }

       
    }
}
