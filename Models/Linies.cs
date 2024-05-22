using panasonic_machine_checker.data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.Models
{

    public class LiniesModel
    {
        public List<Linies>? LiniesList { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class Linies
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? LeaderId { get; set; }

        public int? BuId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("LeaderId")]
        public virtual User UserLeaderId { get; set; } = null!;

        [ForeignKey("BuId")]
        public virtual BU BU { get; set; }

        public virtual Machine Machine { get; set; } = null!;
    }
}
