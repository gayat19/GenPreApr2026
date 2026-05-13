using BankingDALLibrary.Contexts;
using BankingDALLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BankingDALLibrary.Repositories
{
    public abstract class AbstractRepository<K, T> : IRepository<K, T> where T : class
    {
        protected BankingContext context;
        protected AbstractRepository()
        {
            context = new BankingContext();
        }

        public T Create(T item)
        {
            context.Add(item);
            context.SaveChanges();
            return item;
        }

        public T? Delete(K key)
        {
            var item = Get(key);
            if (item == null)
                throw new Exception("No Such item for delete");
            context.Remove(item);
            context.SaveChanges();
            return item;
        }

        public abstract T? Get(K key);
        
        public List<T>? GetAll()
        {
            return context.Set<T>().ToList();
        }

        public T? Update(K key, T item)
        {
            var myItem = Get(key);
            if (myItem == null)
                throw new Exception("No such item for update");
            context.Update(item);
            context.SaveChanges();
            return item;
        }
    }
}
