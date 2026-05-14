using System;
using System.Collections.Generic;

namespace UnderstandingEFCOreTransactionSPAPp.Models;

public partial class Product
{
    public int Productid { get; set; }

    public string Productname { get; set; } = null!;

    public int? Supplierid { get; set; }

    public int? Categoryid { get; set; }

    public string? Quantityperunit { get; set; }

    public decimal? Unitprice { get; set; }

    public short? Unitsinstock { get; set; }

    public short? Unitsonorder { get; set; }

    public short? Reorderlevel { get; set; }

    public short Discontinued { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Supplier? Supplier { get; set; }
}
