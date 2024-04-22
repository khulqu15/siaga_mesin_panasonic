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

    public virtual Case Case { get; set; } = null!;

    public virtual User FilledByNavigation { get; set; } = null!;
}
