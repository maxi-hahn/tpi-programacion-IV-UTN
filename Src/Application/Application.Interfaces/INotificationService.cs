using Application.Dtos.Responses;
namespace Application.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotification(Guid userId, string title, string message, string type);
        Task<IEnumerable<NotificationResponse>> GetUserNotifications(Guid userId);
        Task<int> GetUnreadCount(Guid userId);
        Task MarkAsRead(Guid notificationId, Guid userId);
        Task MarkAllAsRead(Guid userId);
    }
}
