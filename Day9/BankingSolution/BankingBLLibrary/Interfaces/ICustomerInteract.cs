using BankingModelLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BankingBLLibrary.Interfaces
{
    public interface ICustomerInteract
    {
        public Account OpensAccount();
        public void PrintAccountDetails(string accountNumber);

    }
}
