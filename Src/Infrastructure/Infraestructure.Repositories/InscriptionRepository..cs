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
            return await _context.Inscriptions.Where(i => i.UserId == userId).ToListAsync();
        }
        public async Task<IEnumerable<Inscription>> GetByUserIdWithClass(Guid userId)
        {
            return await _context.Inscriptions.Include(i => i.Class)
                .ThenInclude(c => c.Schedules).Where(i => i.UserId == userId && i.IsActive).ToListAsync();
        }
        public async Task<IEnumerable<Inscription>> GetByClassId(Guid classId)
        { 
            return await _context.Inscriptions.Where(i => i.ClassId == classId).ToListAsync(); 
        }
        public async Task<Inscription?> GetByUserAndClass(Guid userId, Guid classId)
        {
            return await _context.Inscriptions.FirstOrDefaultAsync(i => i.UserId == userId && i.ClassId == classId); 
        }
        public async Task Unsubscribe(Inscription inscription)
        {
            inscription.IsActive = false; _context.Inscriptions.Remove(inscription);


        }
        public async Task<bool> ExistsByClassId(Guid classId)
        {
            return await _context.Inscriptions.AnyAsync(i => i.ClassId == classId && i.IsActive);
        }
        public async Task<int> CountActiveByClassId(Guid classId)
        {
            return await _context.Inscriptions.CountAsync(i => i.ClassId == classId && i.IsActive);
        }
    }
}