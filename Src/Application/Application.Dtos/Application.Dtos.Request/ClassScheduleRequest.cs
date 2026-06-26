using Domain.Entity;

namespace Application.Dtos.Request
{
    public class ClassScheduleRequest
    {
        public Day Day { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}