using System;
using System.Collections.Generic;

namespace diplom.Models;

public partial class Eventstatus
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
