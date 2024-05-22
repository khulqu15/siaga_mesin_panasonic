namespace panasonic_machine_checker.data
{
    public class Lini
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int BUId { get; set; }

        public int LeaderId { get; set; }
    
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual BU BULiniId { get; set; } = null!;

        public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();

        public virtual User LeaderLiniId { get; set; } = null!;
    }
}
