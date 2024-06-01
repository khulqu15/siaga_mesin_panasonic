using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.data;

public partial class MachineRepair
{
    public int Id { get; set; }

    public int ScheduleId { get; set; }

    public int ReviewedBy { get; set; }

    public DateTime RepairDate { get; set; }

    public int StatusId { get; set; }

    public string? Corrections { get; set; }

    public string? Result { get; set; }

    [ForeignKey("ReviewedBy")]
    public virtual User ReviewedByNavigation { get; set; } = null!;

    [ForeignKey("ScheduleId")]
    public virtual RepairSchedule Schedule { get; set; } = null!;

    [ForeignKey("StatusId")]
    public virtual StatusMachinerepair Status { get; set; } = null!;
}
