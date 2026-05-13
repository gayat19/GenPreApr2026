using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingModelLibrary
{
    public  class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;


        public DateTime DateOfBirth { get; set; }

        public ICollection<Account>? Accounts { get; set; }

        public override string ToString()
        {
            return $"Customer Id : {Id}\nCustomer Name : {Name}";
        }
    }
}
