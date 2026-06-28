using Domain.Entity;
using Domain.Interface;

namespace Infrastructure.Repositories
{
    public class ScheduleRepository : BaseRepository<Schedule>, IScheduleRepository
    {

        public ScheduleRepository(ApplicationDbContext context):base(context)
        {
        }
        public async Task<Schedule> Create(Schedule schedule)
        {
            await Add(schedule);
            await Save();

            return schedule;
        }

        public async Task<bool> Update(Guid id, Schedule updatedSchedule)
        {
            var schedule = await GetById(id);

            if (schedule == null)
                return false;

            schedule.DayOfWeek = updatedSchedule.DayOfWeek;
            schedule.StartTime = updatedSchedule.StartTime;
            schedule.EndTime = updatedSchedule.EndTime;
            schedule.IsActive = updatedSchedule.IsActive;

            await Save();

            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            var schedule = await GetById(id);

            if (schedule == null)
                return false;

            await base.Delete(schedule);
            await Save();

            return true;
        }
    }
}