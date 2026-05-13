using BankingBLLibrary.Interfaces;
using BankingBLLibrary.Services;
using BankingModelLibrary;
using BankingModelLibrary.Exceptions;

namespace BankingFEApplication
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            ICustomerInteract customerInteract = new CustomerService();
            Console.WriteLine("Please enter the account number");
            string accNum = Console.ReadLine();
            customerInteract.PrintAccountDetails(accNum);
        }
    }
}
