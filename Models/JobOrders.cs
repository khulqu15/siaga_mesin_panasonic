using panasonic_machine_checker.data;
using System.ComponentModel.DataAnnotations;

namespace panasonic_machine_checker.Models
{
    public class JobOrdersModel
    {
        public List<JobOrders>? JobOrdersList { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
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

        public required Case Case { get; set; }
        public required User ScheduledBy { get; set; }
        public required StatusJoborder Status { get; set; }

    }
}
