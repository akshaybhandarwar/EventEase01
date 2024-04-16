using System;
using System.Collections.Generic;

namespace EventEase_01.Models;

public partial class Order
{
    public Guid OrderId { get; set; }

    public Guid? UserId { get; set; }

    public DateTime? OrderDateTime { get; set; }

    public string? PaymentStatus { get; set; }

    public virtual User? User { get; set; }
}

