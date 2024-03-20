using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }


        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public required string EmailAddress { get; set; }

        [Required]
        [StringLength(255)]
        public required string Password { get; set; }

        public DateTime? VerifiedAt { get; set; }

        [Required]
        public required int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual required Roles Roles { get; set; }
    }
}
