using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BankingDALLibrary.Interfaces
{
    public interface IRepository<K,T> where T : class
    {
        public T Create(T item);
        public T? GetAccount(K key);
        public List<T>? GetAccounts();

        public T? Update(K key,T item);
        public T? Delete(K key);

    }
}
