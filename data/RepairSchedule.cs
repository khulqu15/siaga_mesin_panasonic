using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.data;

public partial class RepairSchedule
{
    public int Id { get; set; }

    public int? CaseId { get; set; }

    [Required]
    public required int ScheduledBy { get; set; }

    public DateTime CallDate { get; set; }

    public DateTime RepairDate { get; set; }

    [Required]
    public required int StatusId { get; set; }

    public virtual ICollection<MachineRepair> MachineRepairs { get; set; } = new List<MachineRepair>();

    [ForeignKey("CaseId")]
    public virtual Case Case { get; set; } = null!;

    [ForeignKey("ScheduledBy")]
    public virtual User ScheduledByNavigation { get; set; } = null!;
    
    [ForeignKey("StatusId")]
    public virtual StatusRepairschedule Status { get; set; } = null!;
}
