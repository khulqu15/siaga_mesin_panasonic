using System.ComponentModel.DataAnnotations;

namespace panasonic_machine_checker.Models
{
    public class Roles
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public required string Name { get; set; }
    }
}
