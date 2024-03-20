using System.ComponentModel.DataAnnotations;

namespace panasonic_machine_checker.Models
{
    public class Machines
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        [Required]
        [StringLength(255)]
        public required string Description { get; set; }

        [Required]
        [StringLength(255)]
        public required string Type { get; set;}

        [Required]
        [StringLength(255)]
        public required string Location { get; set; }
    }
}
