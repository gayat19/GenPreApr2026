using System;
using System.Collections.Generic;

namespace UnderstandingEFCOreTransactionSPAPp.Models;

public partial class Tran
{
    public int Id { get; set; }

    public int? Fromacc { get; set; }

    public int? Toacc { get; set; }

    public double? Amount { get; set; }
}
