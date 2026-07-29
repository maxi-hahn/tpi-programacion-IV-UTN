using Application.Dtos.Request;
using Application.Dtos.Responses;
using Domain.Entity;

namespace Application.Mapper
{
    public static class ScheduleMapper
    {
        public static ScheduleResponse ToScheduleResponse(this Schedule schedule, int maxUsers)
        {
            var enrolledUsers = schedule.Inscriptions?
                .Count(i => i.IsActive) ?? 0;

            return new ScheduleResponse
            {
                Id = schedule.Id,
                DayOfWeek = (int)schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,

                EnrolledUsers = enrolledUsers,

                AvailableSpots = Math.Max(0, maxUsers - enrolledUsers),

                IsFull = enrolledUsers >= maxUsers
            };
        }

        public static Schedule ToSchedule(this CreateScheduleRequest request)
        {
            return new Schedule
            {
                Id = Guid.NewGuid(),
                DayOfWeek = (Day)request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsActive = true
            };
        }

        public static Schedule ToSchedule(this UpdateScheduleRequest request)
        {
            return new Schedule
            {
                DayOfWeek = (Day)request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsActive = request.IsActive
            };
        }
    }
}