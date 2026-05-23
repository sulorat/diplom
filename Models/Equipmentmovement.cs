using System;
using System.Collections.Generic;

namespace diplom.Models;

public partial class Equipmentmovement
{
    public int Id { get; set; }

    public int? EquipmentId { get; set; }

    public int? TypeId { get; set; }

    public DateOnly? Fromdate { get; set; }

    public DateOnly? Todate { get; set; }

    public int? Responsibleperson { get; set; }

    public string? Reason { get; set; }

    public string? Fromplace { get; set; }

    public string? Toplace { get; set; }

    public virtual Equipment? Equipment { get; set; }

    public virtual Staff? ResponsiblepersonNavigation { get; set; }

    public virtual Movementtype? Type { get; set; }
}
