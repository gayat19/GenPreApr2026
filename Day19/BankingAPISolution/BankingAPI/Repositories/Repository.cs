using BankingAPI.Contexts;
using BankingAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BankingAPI.Repositories
{
    public class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected  BankingContext _context;
        public Repository(BankingContext context)
        {
            _context = context;
        }

        public T Create(T item)
        {
            _context.Add(item);
            _context.SaveChanges();
            return item;
        }

        public T? Delete(K key)
        {
            var item = Get(key);
            if (item == null)
                throw new Exception("No Such item for delete");
            _context.Remove(item);
            _context.SaveChanges();
            return item;
        }

        public  T? Get(K key)
        {
            var item = _context.Find<T>(key);
            return item;
        }
        
        public List<T>? GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public T? Update(K key, T item)
        {
            var myItem = Get(key);
            if (myItem == null)
                throw new Exception("No such item for update");
            _context.Update(item);
            _context.SaveChanges();
            return item;
        }
    }
}
