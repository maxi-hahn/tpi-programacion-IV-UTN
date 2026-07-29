namespace Application.Dtos.Responses
{
    public class ScheduleResponse
    {
        public Guid Id { get; set; }

        public int DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public bool IsActive { get; set; }
        
        public int EnrolledUsers { get; set; }

        public int AvailableSpots { get; set; }

        public bool IsFull { get; set; }
    }
}