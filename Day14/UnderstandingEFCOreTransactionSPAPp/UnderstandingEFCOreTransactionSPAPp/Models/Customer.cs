using System;
using System.Collections.Generic;

namespace UnderstandingEFCOreTransactionSPAPp.Models;

public partial class Customer
{
    public string Customerid { get; set; } = null!;

    public string Companyname { get; set; } = null!;

    public string? Contactname { get; set; }

    public string? Contacttitle { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Region { get; set; }

    public string? Postalcode { get; set; }

    public string? Country { get; set; }

    public string? Phone { get; set; }

    public string? Fax { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Customerdemographic> Customertypes { get; set; } = new List<Customerdemographic>();
}
