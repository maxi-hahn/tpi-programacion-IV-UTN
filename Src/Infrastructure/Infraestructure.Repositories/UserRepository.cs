using Domain.Entity;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Repositories
{  
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context)
            : base(context)
        {
        }
        public async Task<IEnumerable<Client>> GetClientsByPlanId(Guid planId)
        {
            return await _context.Users
                .OfType<Client>()
                .Where(c => c.Id_Plan == planId)
                .ToListAsync();
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}