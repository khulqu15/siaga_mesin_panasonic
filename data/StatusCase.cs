using System;
using System.Collections.Generic;

namespace panasonic_machine_checker.data;

public partial class StatusCase
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Case> Cases { get; set; } = new List<Case>();
}
