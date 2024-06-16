namespace panasonic_machine_checker.data
{
    public class BU
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public int ManagerId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        
        public virtual ICollection<Lini> Lini { get; set; } = new List<Lini>();

        public virtual ICollection<BuMaintenance> BuMaintenance { get; set; } = new List<BuMaintenance>();

        public virtual User ManagerUserId { get; set; } = null!;
    }
}
