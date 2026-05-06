using BankingBLLibrary.Interfaces;
using BankingBLLibrary.Services;
using BankingModelLibrary.Exceptions;

namespace BankingFEApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ICustomerInteract customerService = new CustomerService();
            try
            {
                var result = customerService.OpensAccount();
                if (result != null)
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
