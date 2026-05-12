using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnderstandingEfCoreApp.Models;

namespace UnderstandingEfCoreApp.Contexts
{
    public class BankingContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=bankingdb;Username=postgres;Password=password");
        }

        public DbSet<Customer>  customers { get; set; }
        public DbSet<Account> accounts { get; set; }
    }
}
