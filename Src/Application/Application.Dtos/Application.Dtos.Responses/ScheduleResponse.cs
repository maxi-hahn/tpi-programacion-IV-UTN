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
        public DateTimeOffset NextClassDate { get; set; }
        public bool IsFull { get; set; }
        public bool IsEnrolled { get; set; }
    }
}