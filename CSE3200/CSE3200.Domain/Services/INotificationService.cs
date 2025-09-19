using CSE3200.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE3200.Domain.Services
{
    public interface INotificationService
    {
        Task CreateNotification(Notification notification);
        Task CreateNotificationForAllUsers(Notification notification, IEnumerable<string> excludeUserIds = null);
        Task<IList<Notification>> GetUserNotifications(string userId);
        Task<int> GetUnreadCount(string userId);
        Task MarkAsRead(Guid notificationId);
        Task MarkAllAsRead(string userId);
    }
}