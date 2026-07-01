using Domain.Entity;
namespace Domain.Interface
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<IEnumerable<Client>> GetClientsByPlanId(Guid planId);
        Task<User?> GetByEmail(string email);
    }
}
