using Domain.Entity;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class InscriptionRepository : BaseRepository<Inscription>, IInscriptionRepository
    {
        public InscriptionRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Inscription>> GetByUserId(Guid userId)
        {
            return await _context.Inscriptions.Include(i => i.Schedule).Where(i => i.UserId == userId).ToListAsync();
        }
        public async Task<IEnumerable<Inscription>> GetByUserIdWithClass(Guid userId)
        {
            return await _context.Inscriptions
                .Include(i => i.Class)
                .Include(i => i.Schedule)
                .Where(i => i.UserId == userId && i.IsActive)
                .ToListAsync();
        }
        public async Task<IEnumerable<Inscription>> GetByScheduleId(Guid scheduleId)
        {
            return await _context.Inscriptions
                .Where(i => i.ScheduleId == scheduleId)
                .ToListAsync();
        }
      public async Task<Inscription?> GetByUserAndSchedule(Guid userId, Guid scheduleId)
    {
        return await _context.Inscriptions
            .Where(i =>
                i.UserId == userId &&
                i.ScheduleId == scheduleId &&
                i.IsActive)
            .OrderByDescending(i => i.InscriptionDate)
            .FirstOrDefaultAsync();
    }
        public async Task Unsubscribe(Inscription inscription)
        {
            inscription.IsActive = false;
            _context.Inscriptions.Update(inscription);
        }
        public async Task<bool> ExistsByScheduleId(Guid scheduleId)
        {
            return await _context.Inscriptions.AnyAsync(i =>
                i.ScheduleId == scheduleId &&
                i.IsActive);
        }
        public async Task<int> CountActiveByScheduleId(Guid scheduleId)
        {
            return await _context.Inscriptions.CountAsync(i =>
                i.ScheduleId == scheduleId &&
                i.IsActive);
        }
        public async Task<IEnumerable<Inscription>> GetByClassId(Guid classId)
        {
            return await _context.Inscriptions
                .Where(i => i.ClassId == classId)
                .ToListAsync();
        }
        public async Task<bool> ExistsByClassId(Guid classId)
        {
            return await _context.Inscriptions.AnyAsync(i =>
                i.ClassId == classId &&
                i.IsActive);
        }
        public async Task<int> CountActiveByClassId(Guid classId)
        {
            return await _context.Inscriptions.CountAsync(i =>
                i.ClassId == classId &&
                i.IsActive);
        }
    }
}