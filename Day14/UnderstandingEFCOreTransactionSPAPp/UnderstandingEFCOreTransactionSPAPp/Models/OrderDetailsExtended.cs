using System;
using System.Collections.Generic;

namespace UnderstandingEFCOreTransactionSPAPp.Models;

public partial class OrderDetailsExtended
{
    public int? Orderid { get; set; }

    public int? Productid { get; set; }

    public string? Productname { get; set; }

    public decimal? Unitprice { get; set; }

    public short? Quantity { get; set; }

    public float? Discount { get; set; }

    public decimal? Extendedprice { get; set; }
}
