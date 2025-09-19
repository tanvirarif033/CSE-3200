using CSE3200.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE3200.Domain.Repositories
{
    public interface INotificationRepository : IRepository<Notification, Guid>
    {
        Task<IList<Notification>> GetUserNotifications(string userId);
        Task<IList<Notification>> GetUnreadUserNotifications(string userId);
        Task<int> GetUnreadCount(string userId);
        Task MarkAsRead(Guid notificationId);
        Task MarkAllAsRead(string userId);
        Task CreateForAllUsers(Notification notification, IEnumerable<string> excludeUserIds = null);
    }
}