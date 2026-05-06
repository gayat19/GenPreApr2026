using UnderstandingOOPSApp.Exceptions;
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
           ICustomerInteract customerService = new CustomerService();
            try
            {
                var result = customerService.OpensAccount();
                if(result != null)
                {
                    Console.WriteLine("Account created");
                    Console.WriteLine(result);
                }
            }
            catch (InvalidContactDetailException ipne)
            {
                Console.WriteLine("Unable to create account since the contact details(Email or phone) you entered seems to be invalid");
                Console.WriteLine(ipne.Message);
            }
            Console.WriteLine("Bye bye");
        }
    }
}
