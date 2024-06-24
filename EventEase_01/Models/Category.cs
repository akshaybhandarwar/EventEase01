using System;
using System.Collections.Generic;

namespace EventEase_01.Models;

public partial class Category
{
    public Guid CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? CategoryDescription { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}

