using System;
using System.Collections.Generic;

namespace panasonic_machine_checker.data;

public partial class StatusMachinerepair
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<MachineRepair> MachineRepairs { get; set; } = new List<MachineRepair>();
}
