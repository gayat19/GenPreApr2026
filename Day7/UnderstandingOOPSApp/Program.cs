using UnderstandingOOPSApp.Interfaces;
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
            new Program().DoBanking();
        }
    }
}
