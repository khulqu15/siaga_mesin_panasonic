using panasonic_machine_checker.Models;
using System;
using System.Collections.Generic;

namespace panasonic_machine_checker.data;



public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Avatar { get; set; }

    public string Phone { get; set; } = null!;

    public DateTime? VerifiedAt { get; set; }

    public int Role { get; set; }

    public virtual ICollection<Case> Cases { get; set; } = new List<Case>();

    public virtual ICollection<JobOrder> JobOrders { get; set; } = new List<JobOrder>();

    public virtual ICollection<Kytform> Kytforms { get; set; } = new List<Kytform>();

    public virtual ICollection<MachineRepair> MachineRepairs { get; set; } = new List<MachineRepair>();

    public virtual ICollection<RepairSchedule> RepairSchedules { get; set; } = new List<RepairSchedule>();

    public virtual Role RoleNavigation { get; set; } = null!;
}
