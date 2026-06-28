using Domain.Interface;
namespace Infrastructure.Repositories
{
    public class ClientRepository : UserRepository, IClientRepository
    {
        public ClientRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
