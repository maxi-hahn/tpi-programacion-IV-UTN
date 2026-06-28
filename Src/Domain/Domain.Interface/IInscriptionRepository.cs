using Domain.Entity;
namespace Domain.Interface
{
    public interface IInscriptionRepository : IBaseRepository<Inscription>
    {
        Task<IEnumerable<Inscription>> GetByUserId(Guid userId);
        Task<IEnumerable<Inscription>> GetByUserIdWithClass(Guid userId);
        Task<IEnumerable<Inscription>> GetByClassId(Guid classId);
        Task<Inscription?> GetByUserAndClass(Guid userId, Guid classId);
        Task<bool> ExistsByClassId(Guid classId);
        Task<int> CountActiveByClassId(Guid classId);
        Task Unsubscribe(Inscription inscription);
    }
}
