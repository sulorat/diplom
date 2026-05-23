using System;
using System.Collections.Generic;

namespace diplom.Models;

public partial class Requeststatus
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Repairrequest> Repairrequests { get; set; } = new List<Repairrequest>();
}
