namespace Application.Dtos.Responses
{
    public class InscriptionResponse
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid ClassId { get; set; }

        public Guid ScheduleId { get; set; }

        public DateTime InscriptionDate { get; set; }

        public bool IsActive { get; set; }
    }
}
