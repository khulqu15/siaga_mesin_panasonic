using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.Models
{
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
        public required string Description { get; set; }
  
        public required Cases Case { get; set; }
        public required Users FilledBy { get; set; }
    }
}
