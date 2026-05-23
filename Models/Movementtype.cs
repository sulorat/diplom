using System;
using System.Collections.Generic;

namespace diplom.Models;

public partial class Movementtype
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Equipmentmovement> Equipmentmovements { get; set; } = new List<Equipmentmovement>();
}
