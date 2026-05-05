using UnderstandingOOPSApp.Interfaces;
using UnderstandingOOPSApp.Models;
using UnderstandingOOPSApp.Repositories;
using UnderstandingOOPSApp.Services;

namespace UnderstandingOOPSApp
{
    internal class Program
    {
        ICustomerInteract customerInteract;
        public Program()
        {
            customerInteract = new CustomerService();
        }
        void DoBanking()
        {
            var account = customerInteract.OpensAccount();
            Console.WriteLine(account);
            Console.WriteLine("Please enter the account you like see");
            string accNum = Console.ReadLine()??"";
            customerInteract.PrintAccountDetails(accNum);
            
        }
        static void Main(string[] args)
        {
            //new Program().DoBanking();
            Account acc1 = new Account("1234", "Ramu",  new DateTime(), "", "9876543210", 2344.4f);
            Account acc2 = new Account("1234", "Ramu", new DateTime(), "", "9876543210", 2344.4f);
            if(acc1==acc2)
                Console.WriteLine("Same");
            else
                Console.WriteLine("Not same");
        }
    }
}
