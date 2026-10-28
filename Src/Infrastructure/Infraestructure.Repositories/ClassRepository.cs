using Domain.Entity;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ClassRepository : BaseRepository<Class>, IClassRepository
    {
        public ClassRepository(ApplicationDbContext context)
        : base(context)
        {
        }

        public override async Task<IEnumerable<Class>> GetAll()
        {
            return await _context.Classes
                .Include(c => c.Schedules.Where(s => !s.IsDeleted)) 
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }
        public override async Task<Class?> GetById(Guid id)
        {
            return await _context.Classes
                .Include(c => c.Schedules.Where(s => !s.IsDeleted)) // <--- FILTRO AQUÍ
                    .ThenInclude(s => s.Inscriptions)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }
    }
}