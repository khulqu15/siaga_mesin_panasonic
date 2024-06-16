using panasonic_machine_checker.data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.Models
{

    public class LiniMaintenancesModel
    {
        public List<LiniMaintenances>? LiniMaintenancesList { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class LiniMaintenances
    {
        [Key]
        public int Id { get; set; }

        public int? LeaderId { get; set; }

        public int? LiniId { get; set; }

        [ForeignKey("LeaderId")]
        public virtual User Leader { get; set; } = null!;

        [ForeignKey("LiniId")]
        public virtual Lini Lini { get; set; } = null!;
    }
}
