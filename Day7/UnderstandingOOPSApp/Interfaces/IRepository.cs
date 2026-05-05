using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnderstandingOOPSApp.Models;

namespace UnderstandingOOPSApp.Interfaces
{
    internal interface IRepository<K,T> where T : class
    {
        public T Create(T item);
        public T? GetAccount(K key);
        public List<T>? GetAccounts();

        public T? Update(K key,T item);
        public T? Delete(K key);

    }
}
