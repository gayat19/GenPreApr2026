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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(c =>
            {
                c.HasKey(c => c.Id);
                c.Property(c => c.DateOfBirth).HasColumnType("timestamp without time zone");
                //seeding
                c.HasData(new Customer() { Id = 101, Name = "Ramu", DateOfBirth = DateTime.Now, Phone = "9876543210", Email = "ramu@gmail.com", Status = "Active" });
            });



            modelBuilder.Entity<Account>(a =>
            {
                a.HasKey(a => a.AccountNumber);

                a.HasOne(a => a.Customer)
                .WithMany(c => c.Accounts)
                .HasForeignKey(a => a.CustomerId)
                .HasConstraintName("FK_Account_Customer")
                .OnDelete(DeleteBehavior.Restrict);

                a.Property(a => a.LastAccessed).HasColumnType("timestamp without time zone");

                a.HasData(new Account()
                {
                    AccountNumber = "0009998877",
                    Balance = 134.3M,
                    CustomerId = 101,
                    LastAccessed = DateTime.Now,
                    Status = "Active"
                });
            });

        }
    }
}
