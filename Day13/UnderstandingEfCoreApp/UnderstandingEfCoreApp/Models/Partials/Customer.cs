using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnderstandingEfCoreApp.Models
{
    public partial class Customer
    {
        public override string ToString()
        {
            return $"Customer Id : {Id}\nName : {Name}";
        }
    }
}
