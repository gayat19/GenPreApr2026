using System;
using System.Collections.Generic;

namespace UnderstandingEFCOreTransactionSPAPp.Models;

public partial class ProductsByCategory
{
    public string? Categoryname { get; set; }

    public string? Productname { get; set; }

    public string? Quantityperunit { get; set; }

    public short? Unitsinstock { get; set; }

    public short? Discontinued { get; set; }
}
