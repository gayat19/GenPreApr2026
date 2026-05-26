using BankingAPI.Contexts;
using BankingAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<T> Create(T item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<T?> Delete(K key)
        {
            var item = await Get(key);
            if (item == null)
                throw new Exception("No Such item for delete");
            _context.Remove(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<T?> Get(K key)
        {
            var item = await _context.FindAsync<T>(key);
            return item;
        }

        public async Task<List<T>?> GetAll()
        {
            return (await _context.Set<T>().ToListAsync());
        }

        public async Task<T?> Update(K key, T item)
        {
            var myItem = await Get(key);
            if (myItem == null)
                throw new Exception("No such item for update");
            _context.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
