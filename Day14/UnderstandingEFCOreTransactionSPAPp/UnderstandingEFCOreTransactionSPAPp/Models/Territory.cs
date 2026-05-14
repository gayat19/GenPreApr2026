using System;
using System.Collections.Generic;

namespace UnderstandingEFCOreTransactionSPAPp.Models;

public partial class Territory
{
    public string Territoryid { get; set; } = null!;

    public string Territorydescription { get; set; } = null!;

    public int Regionid { get; set; }

    public virtual Region Region { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
