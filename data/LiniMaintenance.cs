namespace panasonic_machine_checker.data
{
    public class LiniMaintenance
    {
        public int Id { get; set; }

        public int LiniId { get; set; }

        public int LeaderId { get; set; }

        public virtual Lini Lini { get; set; } = null!;

        public virtual User Leader { get; set; } = null!;
    }
}
