using System;
using System.Collections.Generic;

namespace panasonic_machine_checker.data;

public partial class JobOrder
{
    public int Id { get; set; }

    public int CaseId { get; set; }

    public int ScheduledBy { get; set; }

    public DateTime? ScheduledDate { get; set; }

    public int StatusId { get; set; }

    public string? Description { get; set; }

    public virtual Case Case { get; set; } = null!;

    public virtual User ScheduledByNavigation { get; set; } = null!;

    public virtual StatusJoborder Status { get; set; } = null!;
}
