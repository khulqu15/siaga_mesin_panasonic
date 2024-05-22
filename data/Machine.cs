using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.data;

public partial class Machine
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Location { get; set; } = null!;

    public int? LiniId { get; set; }

    public virtual ICollection<Case> Cases { get; set; } = new List<Case>();

    [ForeignKey("LiniId")]
    public virtual Lini MachineLiniId { get; set; } = null!;
}
