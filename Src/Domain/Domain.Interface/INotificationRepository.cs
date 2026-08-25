using Domain.Entity;
namespace Domain.Interface
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetByUserId(Guid userId);
        Task<int> GetUnreadCount(Guid userId);
    }
}
