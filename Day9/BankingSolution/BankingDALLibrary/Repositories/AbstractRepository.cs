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
        protected Dictionary<K, T> _items;
        public abstract T Create(T item);


        public T? Delete(K key)
        {
            var item = _items[key];
            if (item == null)
                return null;
            return item;
        }

        public  T? GetAccount(K key)
        {
            if(_items.ContainsKey(key))
                return _items[key];
            return null;
        }

        public  List<T>? GetAccounts()
        {
            if(_items.Count == 0) return null;
            var list = _items.Values.ToList();
            return list;

        }
        public T? Update(K key, T item)
        {
            if(_items[key] == null)
                return null;
            _items[key] = item;
            return item;
        }
    }
}
