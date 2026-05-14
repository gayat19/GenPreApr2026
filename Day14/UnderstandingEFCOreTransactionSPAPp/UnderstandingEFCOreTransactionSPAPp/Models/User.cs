using System;
using System.Collections.Generic;

namespace UnderstandingEFCOreTransactionSPAPp.Models;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string? UserPassword { get; set; }

    public string? UserRole { get; set; }
}
