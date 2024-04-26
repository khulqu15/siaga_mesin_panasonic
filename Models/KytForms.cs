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

        public int? Approval { get; set; }

        public virtual Case Case { get; set; } = null!;
        public virtual User FilledBy { get; set; } = null!;
    }
}
