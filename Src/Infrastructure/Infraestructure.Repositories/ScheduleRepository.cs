using Application.Interfaces;
using Domain.Entity;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;
using Trabajop4.Infrastructure;

namespace Infrastructure.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public ScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Schedule>> GetAll()
        {
            return await _context.Schedules.ToListAsync();
        }
        public async Task<Schedule?> GetById(Guid id)
        {
            return await _context.Schedules.FindAsync(id);
        }
        public async Task<List<Schedule>> GetByClassId(Guid classId)
        {
            return await _context.Schedules
                .Where(s => s.Id_Class == classId && s.IsActive)
                .ToListAsync();
        }  

        public async Task<Schedule> Create(Schedule schedule)
        {
            _context.Schedules.Add(schedule);

            await _context.SaveChangesAsync();

            return schedule;
        }

        public async Task<bool> Update(Guid id, Schedule updatedSchedule)
        {
            var schedule = await _context.Schedules.FindAsync(id);

            if (schedule == null)
                return false;

            schedule.DayOfWeek = updatedSchedule.DayOfWeek;
            schedule.StartTime = updatedSchedule.StartTime;
            schedule.EndTime = updatedSchedule.EndTime;
            schedule.IsActive = updatedSchedule.IsActive;
           

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            var schedule = await _context.Schedules.FindAsync(id);

            if (schedule == null)
                return false;

            _context.Schedules.Remove(schedule);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}