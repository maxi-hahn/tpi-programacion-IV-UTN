using Domain.Interface;

namespace Infrastructure.Repositories
{
    public class AdminRepository : UserRepository, IAdminRepository
    {
        public AdminRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
