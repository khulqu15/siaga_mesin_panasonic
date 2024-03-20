using System.ComponentModel.DataAnnotations;

namespace panasonic_machine_checker.Models
{
    public class JobOrders
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CaseId { get; set; }

        [Required]
        public int ScheduledById { get; set; }

        public DateTime? ScheduledDate { get; set; }

        [Required]
        public int StatusId { get; set; }

        public required Cases Case { get; set; }
        public required Users ScheduledBy { get; set; }
        public required Status Status { get; set; }

    }
}
