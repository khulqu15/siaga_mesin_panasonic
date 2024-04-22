using panasonic_machine_checker.data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.Models
{

    public class MachineRepairsModel
    {
        public List<MachineRepairs>? MachineRepairsList { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class MachineRepairs
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ScheduledById { get; set; }

        [Required]
        public int ReviewedById { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        public required DateTime RepairDate { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public required string Corrections { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public required string Result { get; set; }

        public required RepairSchedule ScheduledBy { get; set; }
        public required User ReviewsBy { get; set; }
        public required StatusMachinerepair Status { get; set; }
    }
}
