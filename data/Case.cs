using System;
using System.Collections.Generic;

namespace panasonic_machine_checker.data;

public partial class Case
{
    public int Id { get; set; }

    public int? MachineId { get; set; }

    public int? ReportedById { get; set; }

    public string Description { get; set; } = null!;

    public int? StatusId { get; set; }

    public int? IsApproved { get; set; }

    public int? ApprovedAt { get; set; }
    
    public int? CreatedAt { get; set; }

    public DateTime? DateReported { get; set; }

    public DateTime? DateCompleted { get; set; }

    public virtual ICollection<JobOrder> JobOrders { get; set; } = new List<JobOrder>();

    public virtual ICollection<Kytform> Kytforms { get; set; }

    public virtual ICollection<RepairSchedule> RepairSchedules { get; set; } = new List<RepairSchedule>();

    public virtual Machine Machine { get; set; } = null!;
    
    public virtual User ReportedByNavigation { get; set; } = null!;

    public virtual StatusCase Status { get; set; } = null!;
}
