using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.data;
public class KytMember
{
    public int Id { get; set; }

    public int MemberId { get; set; }

    public int KytId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("MemberId")]
    public virtual User UserMember { get; set; } = null!;
    
    [ForeignKey("KytId")]
    public virtual Kytform Kyform { get; set; } = null!;
}
