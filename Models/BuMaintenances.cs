using panasonic_machine_checker.data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.Models
{

    public class BuMaintenancesModel
    {
        public List<BuMaintenances>? BuMaintenancesList { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class BuMaintenances
    {
        [Key]
        public int Id { get; set; }

        public int? ManagerId { get; set; }

        public int? BuId { get; set; }

        [ForeignKey("ManagerId")]
        public virtual User Manager { get; set; } = null!;

        [ForeignKey("BuId")] 
        public virtual BU BU { get; set; } = null!;
    }
}
