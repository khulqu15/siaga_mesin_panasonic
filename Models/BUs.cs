using panasonic_machine_checker.data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace panasonic_machine_checker.Models
{
    public class BUsModel
    {
        public List<BUs>? BUsList { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class BUs
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
        
        public string? Description { get; set; }
        
        public int? ManagerId { get; set; }
        
        public DateTime? CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("ManagerId")]
        public virtual User UserManagerId { get; set; } = null!;
        
        public virtual Lini Lini { get; set; } = null!;
    }
}
