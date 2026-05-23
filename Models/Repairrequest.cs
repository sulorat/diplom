using System;
using System.Collections.Generic;

namespace diplom.Models;

public partial class Repairrequest
{
    public int Id { get; set; }

    public int? EquipmentId { get; set; }

    public string? Description { get; set; }

    public int? Priority { get; set; }

    public int? StatusId { get; set; }

    public DateOnly? Createdat { get; set; }

    public bool? Isdeleted { get; set; }

    public virtual Equipment? Equipment { get; set; }

    public virtual Priority? PriorityNavigation { get; set; }

    public virtual Requeststatus? Status { get; set; }
}
