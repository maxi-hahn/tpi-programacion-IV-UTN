using Domain.Entity;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;
using Trabajop4.Infrastructure;

namespace Infrastructure.Repositories
{
    public class InscriptionRepository : IInscriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public InscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Inscription>> GetAll()
        {
            return await _context.Inscriptions.ToListAsync();
        }
        public async Task<Inscription?> GetById(Guid id)
        {
            return await _context.Inscriptions
                .FirstOrDefaultAsync(i => i.Id == id);
        }
        public async Task<IEnumerable<Inscription>> GetByClassId(Guid classId)
        {
            return await _context.Inscriptions
                .Where(i => i.ClassId == classId)
                .ToListAsync();
        }

        public async Task<Inscription?> GetByUserAndClass(Guid userId, Guid classId)
        {
            return await _context.Inscriptions
                .FirstOrDefaultAsync(i => i.UserId == userId && i.ClassId == classId);
        }

        public async Task Add(Inscription inscription)
        {
            await _context.Inscriptions.AddAsync(inscription);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}