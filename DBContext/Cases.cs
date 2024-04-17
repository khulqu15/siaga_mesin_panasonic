using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.DBContext
{
    public class Cases
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public required string Description { get; set; }

        [Required]
        public int MachineId { get; set; }

        [Required]
        public int ReportedById { get; set; }

        [Required]
        public int StatusId { get; set; }

        public DateTime? DateReported { get; set; }
        public DateTime? DateCompleted { get; set; }
    }
}
