using System;
using System.Collections.Generic;

namespace EventEase_01.Models;

public partial class Venue
{
    public Guid VenueId { get; set; }

    public string? VenueName { get; set; }

    public string? VenueAddress { get; set; }

    public int? VenueCapacity { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
