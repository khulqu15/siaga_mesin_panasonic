namespace panasonic_machine_checker.data
{
    public class BuMaintenance
    {
        public int Id { get; set; }

        public int BUId { get; set; }

        public int ManagerId { get; set; }

        public virtual BU BU { get; set; } = null!;

        public virtual User Manager { get; set; } = null!;
    }
}
