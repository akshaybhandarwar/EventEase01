using System;
using System.Collections.Generic;

namespace EventEase_01.Models;

public partial class Seat
{
    public Guid SeatId { get; set; }

    public Guid? VenueId { get; set; }

    public string? SeatNumber { get; set; }

    public bool? SeatAvailability { get; set; }

    public bool? SeatBookingStatus { get; set; }

    public decimal? SeatPrice { get; set; }

    public virtual Venue? Venue { get; set; }
}
