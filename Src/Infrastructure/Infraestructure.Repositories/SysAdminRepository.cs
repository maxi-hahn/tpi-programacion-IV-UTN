using Domain.Interface;
namespace Infrastructure.Repositories
{
    public class SysAdminRepository : UserRepository, ISysAdminRepository
    {
        public SysAdminRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}



