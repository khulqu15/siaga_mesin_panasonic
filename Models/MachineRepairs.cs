using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.Models
{
    public class MachineRepairs
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ScheduledById { get; set; }

        [Required]
        public int ReviewedById { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        public required DateTime RepairDate { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public required string Corrections { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public required string Result { get; set; }

        public required Cases ScheduledBy { get; set; }
        public required Users ReviewsBy { get; set; }
        public required Status Status { get; set; }
    }
}
