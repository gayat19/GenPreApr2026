using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BankingAPI.Interfaces
{
    public interface IRepository<K,T> where T : class
    {
        public Task<T> Create(T item);
        public Task<T?> Get(K key);
        public Task<List<T>?> GetAll();

        public Task<T?> Update(K key,T item);
        public Task<T?> Delete(K key);

    }
}
