using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnderstandingEfCoreApp.Models
{
    public partial class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [Column(TypeName = "timestamp without time zone")]
        public DateTime DateOfBirth { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
