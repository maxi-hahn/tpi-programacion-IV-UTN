namespace Domain.Entity
{
    public class Inscription
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid ClassId { get; set; }
        public Class? Class { get; set; }

        public DateTime InscriptionDate { get; set; }
        public Guid ScheduleId { get; set; }

        public Schedule? Schedule { get; set; }
        public bool IsActive { get; set; }
    }
}