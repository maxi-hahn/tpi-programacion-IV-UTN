namespace Application.Dtos.Request
{
    public class InscriptionRequest
    {
        public Guid UserId { get; set; }
        public Guid ClassId { get; set; }
        public Guid ScheduleId { get; set; }
    }
}