using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingModelLibrary.Misc
{
    public static class StringExt
    {
        public static int CountWords(this string str,char delimiter)
        {
            //space as delimiter
            var words = str.Split(delimiter);
            var result = words.Count();
            return result;
        }
    }
}
