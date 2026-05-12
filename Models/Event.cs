using System;
using System.Collections.Generic;

namespace diplom.Models;

public partial class Event
{
    public int Id { get; set; }

    public int? EquipmentId { get; set; }

    public DateOnly Dateof { get; set; }

    public string Description { get; set; } = null!;

    public int? EventstatusId { get; set; }

    public virtual Equipment? Equipment { get; set; }

    public virtual Eventstatus? Eventstatus { get; set; }
}
