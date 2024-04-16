using System;
using System.Collections.Generic;

namespace EventEase_01.Models;

public partial class Ticket
{
    public Guid TicketId { get; set; }

    public Guid? EventId { get; set; }

    public decimal? TicketPrice { get; set; }

    public int? TicketAvailability { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Event? Event { get; set; }
}
