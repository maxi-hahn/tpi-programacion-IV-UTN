namespace Application.Dtos.Responses
{
    public class ClassDetailResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Max_Users { get; set; }
        public int CurrentInscriptions { get; set; }
        public List<ScheduleResponse> Schedules { get; set; } = new();
        public List<ClientInfoResponse> Clients { get; set; } = new();
    }

    public class ClientInfoResponse
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public Guid ScheduleId { get; set; }

        public ScheduleResponse Schedule { get; set; } = null!;
    }
}