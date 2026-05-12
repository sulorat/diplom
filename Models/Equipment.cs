using System;
using System.Collections.Generic;

namespace diplom.Models;

public partial class Equipment
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly? Dateoflastcheck { get; set; }

    public string Place { get; set; } = null!;

    public int? StatusId { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual Status? Status { get; set; }
}
