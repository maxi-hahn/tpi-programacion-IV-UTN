namespace Application.Dtos.Responses
{
    public class MyInscriptionResponse
    {
        public Guid InscriptionId { get; set; }
        public Guid ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public Guid ScheduleId { get; set; }
        public ScheduleResponse Schedule { get; set; } = null!;
        public DateTimeOffset InscriptionDate { get; set; }
        public DateTimeOffset ClassDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsConsumed { get; set; }
    }
}
