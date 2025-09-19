using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CSE3200.Web.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public async Task JoinNotificationGroup()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            }
        }

        public async Task LeaveNotificationGroup()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
            }
        }

        public override async Task OnConnectedAsync()
        {
            await JoinNotificationGroup();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await LeaveNotificationGroup();
            await base.OnDisconnectedAsync(exception);
        }
    }
}