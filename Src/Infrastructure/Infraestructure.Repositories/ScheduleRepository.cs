using Domain.Entity;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ScheduleRepository : BaseRepository<Schedule>, IScheduleRepository
    {

        public ScheduleRepository(ApplicationDbContext context):base(context)
        {
        }

        public override async Task<IEnumerable<Schedule>> GetAll()
        {
            return await _context.Schedules
                .Include(s => s.Class)
                .Include(s => s.Inscriptions)
                .ToListAsync();
        }
        public async Task<IEnumerable<Schedule>> GetByClassId(Guid classId)
        {
            return await _context.Schedules
                .Where(s => s.Id_Class == classId)
                .ToListAsync();
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