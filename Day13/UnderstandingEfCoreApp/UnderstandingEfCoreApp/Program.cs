using UnderstandingEfCoreApp.Contexts;
using UnderstandingEfCoreApp.Models;

namespace UnderstandingEfCoreApp
{
    internal class Program
    {
        BankingContext bankingContext;

        public Program()
        {
            bankingContext = new BankingContext();
        }
        void InsertCustomer()
        {
            Customer customer = new Customer() { Name = "Ramu", DateOfBirth =DateTime.Now, Phone = "9876543210", Email = "ramu@gmail.com", Status = "Active" };
            bankingContext.customers.Add(customer);
            Console.WriteLine("The state of the object before save change and after adding "+bankingContext.Entry<Customer>(customer).State);
            bankingContext.SaveChanges();
            Console.WriteLine("The state of the object after save change and after adding " + bankingContext.Entry<Customer>(customer).State);
        }

        void UpdateCustomer()
        {
            Customer customer = new Customer() { Id=1,Name = "Somu", DateOfBirth = DateTime.Now, Phone = "9876543210", Email = "ramu@gmail.com", Status = "Active" };
            bankingContext.customers.Update(customer);
            Console.WriteLine("The state of the object before save change and after adding " + bankingContext.Entry<Customer>(customer).State);
            bankingContext.SaveChanges();
            Console.WriteLine("The state of the object after save change and after adding " + bankingContext.Entry<Customer>(customer).State);
        }

        void GetAllCustomers()
        {
            var customers = bankingContext.customers;
            foreach(var custoemr in customers)
                Console.WriteLine(custoemr);
        }
        static void Main(string[] args)
        {
           Program program = new Program();
            program.GetAllCustomers();
        }
    }
}
