using CSE3200.Domain;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CSE3200.Infrastructure.Repositories
{
    public class NotificationRepository : Repository<Notification, Guid>, INotificationRepository
    {
        protected readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IList<Notification>> GetUserNotifications(string userId)
        {
            return await GetDynamicAsync(
                filter: n => n.UserId == userId,
                orderBy: "CreatedDate DESC",
                include: null,
                isTrackingOff: false
            );
        }

        public async Task<IList<Notification>> GetUnreadUserNotifications(string userId)
        {
            return await GetDynamicAsync(
                filter: n => n.UserId == userId && !n.IsRead,
                orderBy: "CreatedDate DESC",
                include: null,
                isTrackingOff: false
            );
        }

        public async Task<int> GetUnreadCount(string userId)
        {
            return await GetCountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAsRead(Guid notificationId)
        {
            var notification = await GetByIdAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsRead(string userId)
        {
            var unreadNotifications = await GetDynamicAsync(
                filter: n => n.UserId == userId && !n.IsRead,
                orderBy: null,
                include: null,
                isTrackingOff: false
            );

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreateForAllUsers(Notification templateNotification, IEnumerable<string> excludeUserIds = null)
        {
            // Convert string user IDs to GUIDs for comparison
            var excludeGuids = excludeUserIds?
                .Where(id => Guid.TryParse(id, out _))
                .Select(id => Guid.Parse(id))
                .ToList() ?? new List<Guid>();

            // Get all users except excluded ones
            var users = await _context.Users
                .Where(u => excludeGuids.Count == 0 || !excludeGuids.Contains(u.Id))
                .ToListAsync();

            var notifications = users.Select(user => new Notification
            {
                Id = Guid.NewGuid(),
                Title = templateNotification.Title,
                Message = templateNotification.Message,
                Type = templateNotification.Type,
                IsRead = false,
                CreatedDate = DateTime.UtcNow,
                RelatedEntityId = templateNotification.RelatedEntityId,
                RelatedEntityType = templateNotification.RelatedEntityType,
                UserId = user.Id.ToString() // Convert Guid to string
            });

            await _context.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();
        }

        // Helper method to resolve the ambiguous GetDynamicAsync call
        private async Task<IList<Notification>> GetDynamicAsync(
            Expression<Func<Notification, bool>> filter = null,
            string orderBy = null,
            Func<IQueryable<Notification>, IIncludableQueryable<Notification, object>> include = null,
            bool isTrackingOff = false)
        {
            return await base.GetDynamicAsync(filter, orderBy, include, isTrackingOff);
        }
    }
}