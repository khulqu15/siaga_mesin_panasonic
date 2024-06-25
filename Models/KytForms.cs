using panasonic_machine_checker.data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.Models
{

    public class KytFormsModel
    {
        public List<KytForms>? KytFormsList { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
    public class KytForms
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CaseId { get; set; }

        [Required]
        public int FilledById { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public required string PotentialHazard { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string? Description { get; set; }
        public string? Action { get; set; }
        public string? DangerousMode { get; set; }
        public string? PrepareProcess { get; set; }
        public string? PreparePrediction { get; set; }
        public string? PrepareAction { get; set; }
        public string? MainProcess { get; set; }
        public string? MainPrediction { get; set; }
        public string? MainAction { get; set; }
        public string? ConfirmProcess { get; set; }
        public string? ConfirmPrediction { get; set; }
        public string? ConfirmAction { get; set; }
        public DateTime? ApproveAt { get; set; }
        public int? Approval { get; set; }
        public virtual Case Case { get; set; } = null!;
        public virtual User FilledBy { get; set; } = null!;
    }
}
