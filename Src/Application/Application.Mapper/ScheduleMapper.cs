using Application.Dtos.Request;
using Application.Dtos.Responses;
using Domain.Entity;

namespace Application.Mapper
{
    public static class ScheduleMapper
    {
        public static ScheduleResponse ToScheduleResponse(
        this Schedule schedule,
        int maxUsers,
        Guid? userId = null)
        {
            var nextClassDate = GetNextOccurrence(schedule.DayOfWeek, schedule.StartTime);

            // Null check para Inscriptions
            var inscriptions = schedule.Inscriptions ?? new List<Inscription>();

            var enrolledUsers = inscriptions
                .Count(i => i.IsActive && i.ClassDate.Date == nextClassDate.Date);

            var isEnrolled = userId != null &&
                             inscriptions.Any(i =>
                                 i.UserId == userId &&
                                 i.IsActive &&
                                 i.ClassDate.Date == nextClassDate.Date);

            return new ScheduleResponse
            {
                Id = schedule.Id,
                DayOfWeek = (int)schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                IsActive = schedule.IsActive,
                NextClassDate = new DateTimeOffset(nextClassDate, TimeZoneInfo.Local.GetUtcOffset(nextClassDate)),
                EnrolledUsers = enrolledUsers,
                AvailableSpots = Math.Max(0, maxUsers - enrolledUsers),
                IsFull = enrolledUsers >= maxUsers,
                IsEnrolled = isEnrolled
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

        // Método privado para calcular la próxima ocurrencia
        private static DateTime GetNextOccurrence(Day dayOfWeek, TimeOnly startTime)
        {
            var now = DateTime.Now;

            DayOfWeek targetDayOfWeek = dayOfWeek switch
            {
                Day.Lunes => DayOfWeek.Monday,
                Day.Martes => DayOfWeek.Tuesday,
                Day.Miercoles => DayOfWeek.Wednesday,
                Day.Jueves => DayOfWeek.Thursday,
                Day.Viernes => DayOfWeek.Friday,
                Day.Sabado => DayOfWeek.Saturday,
                Day.Domingo => DayOfWeek.Sunday,
                _ => DayOfWeek.Monday
            };

            var currentDay = now.DayOfWeek;
            var daysUntilTarget = ((int)targetDayOfWeek - (int)currentDay + 7) % 7;
            var nextDate = now.Date.AddDays(daysUntilTarget);

            if (daysUntilTarget == 0 && now.TimeOfDay > startTime.ToTimeSpan())
            {
                nextDate = nextDate.AddDays(7);
            }

            var result = nextDate.Add(startTime.ToTimeSpan());

            // Log para debug
            Console.WriteLine($"Day enum: {dayOfWeek}, Target DayOfWeek: {targetDayOfWeek}, Current: {currentDay}, Days diff: {daysUntilTarget}, Next date: {result}");

            return result;
        }
    }
}