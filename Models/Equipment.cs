using System;
using System.Collections.Generic;

namespace diplom.Models;

public partial class Equipment
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly? Dateoflastcheck { get; set; }

    public string Place { get; set; } = null!;

    public int? Workhours { get; set; }

    public int? Productioncapacity { get; set; }

    public bool? Isdeleted { get; set; }

    public DateOnly? Enddate { get; set; }

    public int? StatusId { get; set; }

    public string? Imagepath { get; set; }

    public virtual ICollection<Equipmentmovement> Equipmentmovements { get; set; } = new List<Equipmentmovement>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Repairrequest> Repairrequests { get; set; } = new List<Repairrequest>();

    public virtual Status? Status { get; set; }
}
