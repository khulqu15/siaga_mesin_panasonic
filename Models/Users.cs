using panasonic_machine_checker.data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.Models
{

    public class UsersModel
    {
        public List<Users>? UserList { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

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
        public required string Email { get; set; }

        [Required]
        [StringLength(255)]
        public required string Password { get; set; }

        public string? Avatar {  get; set; }

        [Required]
        [StringLength(255)]
        public required string Phone { get; set; }

        public DateTime? VerifiedAt { get; set; }

        [Required]
        public required int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role RoleNavigation { get; set; } = null!;

    }
}
