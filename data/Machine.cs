using System;
using System.Collections.Generic;

namespace panasonic_machine_checker.data;

public partial class Machine
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Location { get; set; } = null!;

    public virtual ICollection<Case> Cases { get; set; } = new List<Case>();
}
