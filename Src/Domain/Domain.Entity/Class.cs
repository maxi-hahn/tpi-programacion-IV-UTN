namespace Domain.Entity
{
    public class Class
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Max_Users { get; set; }
        public bool IsActive { get; set; } = true;
        public List<Schedule> Schedules { get; set; } = new();

        public List<Inscription> Inscriptions { get; set; } = new();
        public bool IsDeleted { get; set; } = false;  // NUEVO
        public Class() { }
    }
}