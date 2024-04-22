using System.ComponentModel.DataAnnotations;

namespace panasonic_machine_checker.Models
{
    
    public class RolesModels
    {
        public List<Roles>? RolesList { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class Roles
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public required string Name { get; set; }
    }
}
