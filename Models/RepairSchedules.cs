using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.Models
{
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

        public required Cases Case { get; set; }
        public required Users SchedulesBy { get; set; }
        public required Status Status { get; set; }
    }
}
