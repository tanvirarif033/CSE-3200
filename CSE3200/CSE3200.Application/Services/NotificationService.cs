using CSE3200.Domain.Entities;
using CSE3200.Domain.Repositories;
using CSE3200.Domain.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE3200.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository notificationRepository,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task CreateNotification(Notification notification)
        {
            try
            {
                // Use the repository's Add method which should handle saving changes
                await _notificationRepository.AddAsync(notification);
                // If your repository doesn't auto-save, you'll need to modify the repository interface
                // to include a SaveChanges method or use a Unit of Work pattern
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                throw;
            }
        }

        public async Task CreateNotificationForAllUsers(Notification notification, IEnumerable<string> excludeUserIds = null)
        {
            try
            {
                await _notificationRepository.CreateForAllUsers(notification, excludeUserIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notifications for all users");
                throw;
            }
        }

        public async Task<IList<Notification>> GetUserNotifications(string userId)
        {
            return await _notificationRepository.GetUserNotifications(userId);
        }

        public async Task<int> GetUnreadCount(string userId)
        {
            return await _notificationRepository.GetUnreadCount(userId);
        }

        public async Task MarkAsRead(Guid notificationId)
        {
            await _notificationRepository.MarkAsRead(notificationId);
        }

        public async Task MarkAllAsRead(string userId)
        {
            await _notificationRepository.MarkAllAsRead(userId);
        }
    }
}