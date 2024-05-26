using panasonic_machine_checker.data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.Models
{

    public class CasesModel
    {
        public List<Cases>? CasesList { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
    public class Cases
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public required string Description { get; set; }
        
        public int? MachineId { get; set; }
     
        public int? ReportedById { get; set; }

        public int? StatusId { get; set; }

        public int? ApprovedAt { get; set; }

        public int? CreatedAt { get; set; }

        public int? IsApproved { get; set; }

        public DateTime? DateReported { get; set; }
        public DateTime? DateCompleted { get; set; }

        [ForeignKey("MachineId")]
        public virtual Machine Machine { get; set; } = null!;

        [ForeignKey("ReportedById")]
        public virtual User ReportedByNavigation { get; set; } = null!;

        [ForeignKey("StatusId")]
        public virtual StatusCase Status { get; set; } = null!;

        public virtual ICollection<RepairSchedule> RepairSchedule { get; set; }

        public virtual ICollection<Kytform> Kytforms { get; set; }

        public virtual BU BU { get; set; }

        public virtual Lini Lini { get; set; }

        public virtual User ClosedData { get; set; } = null!;
    }
}
