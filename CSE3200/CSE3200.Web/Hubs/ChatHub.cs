using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CSE3200.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);

                if (Context.User.IsInRole("Admin"))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
                }
            }

            await base.OnConnectedAsync();
        }

        public async Task SendMessageToUser(Guid receiverId, string content)
        {
            var senderId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var isAdmin = Context.User.IsInRole("Admin");

            // Send to receiver
            await Clients.Group(receiverId.ToString()).SendAsync("ReceiveMessage", new
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsFromAdmin = isAdmin
            });

            // If admin sends message, also send to admin group for real-time update
            if (isAdmin)
            {
                await Clients.Group("Admins").SendAsync("AdminMessageSent", new
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = content,
                    SentAt = DateTime.UtcNow
                });
            }
        }
    }
}