using System;
using System.Collections.Generic;

namespace diplom.Models;

public partial class Staff
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Post { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<Equipmentmovement> Equipmentmovements { get; set; } = new List<Equipmentmovement>();
}
