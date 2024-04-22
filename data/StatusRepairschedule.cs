using System;
using System.Collections.Generic;

namespace panasonic_machine_checker.data;

public partial class StatusRepairschedule
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<RepairSchedule> RepairSchedules { get; set; } = new List<RepairSchedule>();
}
