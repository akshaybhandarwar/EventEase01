using System;
using System.Collections.Generic;

namespace EventEase_01.Models;

public partial class Booking
{
    public Guid BookingId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? TicketId { get; set; }

    public int? NumberOfBookings { get; set; }

    public DateTime? BookingDateTime { get; set; }

    public virtual Ticket? Ticket { get; set; }

    public virtual User? User { get; set; }
}
