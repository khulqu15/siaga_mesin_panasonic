using panasonic_machine_checker.data;
using System;
using System.Collections.Generic;

namespace panasonic_machine_checker.data;

public partial class Kytform
{
    public int Id { get; set; }

    public int CaseId { get; set; }

    public int FilledBy { get; set; }

    public string? PotentialHazard { get; set; }

    public string? Description { get; set; }

    public string? Action { get; set; }
    public string? DangerousMode { get; set; }
    public string? PrepareProcess { get; set; }
    public string? PreparePrediction { get; set; }
    public string? MainProcess { get; set; }
    public string? MainPrediction { get; set; }
    public string? ConfirmProcess { get; set; }
    public string? ConfirmPrediction { get; set; }
    public int? Approval { get; set; }

    public DateTime? ApproveAt { get; set; }

    public virtual Case Case { get; set; } = null!;
    public virtual ICollection<KytMember> Member { get; set; } = new List<KytMember>();

    public virtual User FilledByNavigation { get; set; } = null!;
}
