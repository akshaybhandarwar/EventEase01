using System;
using System.Collections.Generic;

namespace EventEase_01.Models;

public partial class Event
{
    public Guid EventId { get; set; }

    public string? EventName { get; set; }

    public string? EventDescription { get; set; }

    public DateTime? EventDate { get; set; }

    public Guid? VenueId { get; set; }
    //public string VenueName { get; set; }   
    public Guid? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual Venue? Venue { get; set; }
}