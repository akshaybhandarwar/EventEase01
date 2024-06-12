using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace EventEase_01.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public string? UserName { get; set; }

    public int? UserAge { get; set; }

    public string? UserEmail { get; set; }

    public string? PasswordHash { get; set; }

    public string? PasswordSalt { get; set; }

    public string? UserRole { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
