using BankingAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAPI.Contexts
{
    public class BankingContext : DbContext
    {

        public BankingContext(DbContextOptions dbContextOptions) : base(dbContextOptions) 
        {
            
        }

        public DbSet<Account>  Accounts { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<SavingAccount> SavingsAccounts { get; set; }

        public DbSet<CurrentAccount> CurrentAccounts { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(c =>
            {
                c.HasKey(c => c.Id).HasName("PK_CustomerId");
                c.Property(c => c.DateOfBirth).HasColumnType("timestamp without time zone");
                //seeding
                c.HasData(new Customer() { Id = 101, Name = "Ramu", Phone = "9876543210", DateOfBirth= new DateTime(2000,12,12), Email = "ramu@gmail.com", Status = "Active" });
            });



            modelBuilder.Entity<Account>(a =>
            {
            a.HasKey(a => a.AccountNumber).HasName("PK_AccountNumber");

            a.HasOne(a => a.Customer)
            .WithMany(c => c.Accounts)
            .HasForeignKey(a => a.CustomerId)
            .HasConstraintName("FK_Account_Customer")
            .OnDelete(DeleteBehavior.Restrict);

            a.HasDiscriminator<string>("AccountType")
            .HasValue<SavingAccount>("Savings Account")
            .HasValue<CurrentAccount>("Current Account");




                a.HasData(new Account()
                {
                    AccountNumber = "000999887711",
                    Balance = 12343.4f,
                    CustomerId = 101,
                    Status = "Active"
                });
            });

            modelBuilder.Entity<SavingAccount>();
            modelBuilder.Entity<CurrentAccount>();

            modelBuilder.Entity<User>().HasKey(u => u.Username).HasName("PK_Username");

            modelBuilder.Entity<Customer>()
                .HasOne(c=>c.User)
                .WithOne(u=>u.Customer)
                .HasForeignKey<Customer>(c=>c.Username)
                .HasConstraintName("FK_Customer_user")
                .OnDelete(DeleteBehavior.Restrict); ;


        }
    }
}
