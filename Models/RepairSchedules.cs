using panasonic_machine_checker.data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.Models
{

    public class RepairSchedulesModel
    {
        public List<RepairSchedules>? RepairSchedulesList { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class RepairSchedules
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CaseId { get; set; }

        [Required]
        public int ScheduledById { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        public required DateTime RepairDate { get; set; }

        [Required]
        public required DateTime CallDate { get; set; }

        public virtual Case Case { get; set; } = null!;
        public virtual User ScheduledByNavigation { get; set; } = null!;
        public virtual StatusRepairschedule Status { get; set; } = null!;
    }
}
