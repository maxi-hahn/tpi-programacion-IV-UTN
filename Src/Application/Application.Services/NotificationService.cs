using Application.Dtos.Responses;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entity;
using Domain.Interface;

namespace Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepo;

        public NotificationService(INotificationRepository notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public async Task CreateNotification(Guid userId, string title, string message, string type)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            await _notificationRepo.Add(notification);
            await _notificationRepo.Save();
        }

        public async Task<IEnumerable<NotificationResponse>> GetUserNotifications(Guid userId)
        {
            var notifications = await _notificationRepo.GetByUserId(userId);

            return notifications.Select(n => new NotificationResponse
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            });
        }

        public async Task<int> GetUnreadCount(Guid userId)
        {
            return await _notificationRepo.GetUnreadCount(userId);
        }

        public async Task MarkAsRead(Guid notificationId, Guid userId)
        {
            var notification = await _notificationRepo.GetById(notificationId)
                ?? throw new NotFoundException("Notificación no encontrada.");

            if (notification.UserId != userId)
                throw new ForbiddenException("No tenés permiso para modificar esta notificación.");

            notification.IsRead = true;
            await _notificationRepo.Update(notification);
            await _notificationRepo.Save();
        }

        public async Task MarkAllAsRead(Guid userId)
        {
            var notifications = await _notificationRepo.GetByUserId(userId);
            var unread = notifications.Where(n => !n.IsRead).ToList();

            foreach (var n in unread)
            {
                n.IsRead = true;
                await _notificationRepo.Update(n);
            }

            await _notificationRepo.Save();
        }
    }
}
