using System.ComponentModel.DataAnnotations;

namespace panasonic_machine_checker.Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public required string Name { get; set; }
    }
}
