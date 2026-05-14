using System;
using System.Collections.Generic;

namespace UnderstandingEFCOreTransactionSPAPp.Models;

public partial class AlphabeticalListOfProduct
{
    public int? Productid { get; set; }

    public string? Productname { get; set; }

    public int? Supplierid { get; set; }

    public int? Categoryid { get; set; }

    public string? Quantityperunit { get; set; }

    public decimal? Unitprice { get; set; }

    public short? Unitsinstock { get; set; }

    public short? Unitsonorder { get; set; }

    public short? Reorderlevel { get; set; }

    public short? Discontinued { get; set; }

    public string? Categoryname { get; set; }
}
