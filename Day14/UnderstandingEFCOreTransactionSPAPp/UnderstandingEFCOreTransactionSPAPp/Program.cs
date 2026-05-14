using Microsoft.EntityFrameworkCore;
using UnderstandingEFCOreTransactionSPAPp.Models;

namespace UnderstandingEFCOreTransactionSPAPp
{
    internal class Program
    {
        readonly DummydbContext _context;
        Program()
        {
            _context = new DummydbContext();
        }

        void TransactWithTransactioninDatabase()
        {
            Account fc = new Account() { Aacno = 2 };
            Account tc = new Account() { Aacno = 3};
            float amount = 1500;
            int tran_id = 7;
            fc = _context.Accounts.FirstOrDefault(a => a.Aacno == fc.Aacno);
            tc = _context.Accounts.FirstOrDefault(a => a.Aacno == tc.Aacno);
            using var transaction = _context.Database.BeginTransaction();
            try
            {

                _context.Database.ExecuteSqlInterpolated($"call add_trans({tran_id},{fc.Aacno},{tc.Aacno},{amount})");
                if (fc.Balance - amount <= 0)
                    throw new Exception("Insuffienct balance");
                _context.Database.ExecuteSqlInterpolated($"call update_account({fc.Aacno},{fc.Balance-amount})");
                _context.Database.ExecuteSqlInterpolated($"call update_account({tc.Aacno},{tc.Balance + amount})");
                transaction.Commit();
                Console.WriteLine("Transaction successfull");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex.Message);
            }
        }
        void AddAccountUsingSP()
        {
            Account account = new Account() { Aacno = 5, Balance = 1233.3f };
            //call add_account(4,3243);
            _context.Database.ExecuteSqlInterpolated($"call add_account({account.Aacno},{account.Balance});");
            Console.WriteLine("Account Created");
        }
        static void Main(string[] args)
        {
            new Program().TransactWithTransactioninDatabase();
        }
    }
}
