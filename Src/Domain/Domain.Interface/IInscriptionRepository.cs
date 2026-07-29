using Domain.Entity;
namespace Domain.Interface
{
    public interface IInscriptionRepository : IBaseRepository<Inscription>
    {
        Task<IEnumerable<Inscription>> GetByUserId(Guid userId);

        Task<IEnumerable<Inscription>> GetByUserIdWithClass(Guid userId);

        Task<IEnumerable<Inscription>> GetByScheduleId(Guid scheduleId);

        Task<Inscription?> GetByUserAndSchedule(Guid userId, Guid scheduleId);

        Task<bool> ExistsByScheduleId(Guid scheduleId);

        Task<int> CountActiveByScheduleId(Guid scheduleId);

        Task<IEnumerable<Inscription>> GetByClassId(Guid classId);

        Task<bool> ExistsByClassId(Guid classId);

        Task<int> CountActiveByClassId(Guid classId);

        Task Unsubscribe(Inscription inscription);
    }
}
