using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MediatR;
using CSE3200.Application.Features.Chat.Commands;
using CSE3200.Infrastructure;

namespace CSE3200.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;

        public ChatHub(IMediator mediator, ApplicationDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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

        public async Task SendMessageToUser(string receiverId, string content)
        {
            try
            {
                var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return;

                var senderId = Guid.Parse(userIdClaim.Value);
                var isAdmin = Context.User.IsInRole("Admin");
                var receiverGuid = Guid.Parse(receiverId);

                // Get sender name from database
                var senderName = await GetUserDisplayNameAsync(senderId, isAdmin);

                // Save message to database
                var messageId = await _mediator.Send(new CreateChatMessageCommand
                {
                    SenderId = senderId,
                    ReceiverId = receiverGuid,
                    Content = content,
                    IsFromAdmin = isAdmin
                });

                // Send to receiver
                await Clients.Group(receiverId).SendAsync("ReceiveMessage", new
                {
                    Id = messageId.ToString(),
                    SenderId = senderId.ToString(),
                    SenderName = senderName,
                    ReceiverId = receiverId,
                    Content = content,
                    SentAt = DateTime.UtcNow,
                    IsFromAdmin = isAdmin
                });

                // If admin sent to user, also send to admin's own connection to update UI
                if (isAdmin)
                {
                    await Clients.Caller.SendAsync("ReceiveMessage", new
                    {
                        Id = messageId.ToString(),
                        SenderId = senderId.ToString(),
                        SenderName = senderName,
                        ReceiverId = receiverId,
                        Content = content,
                        SentAt = DateTime.UtcNow,
                        IsFromAdmin = isAdmin
                    });
                }

                Console.WriteLine($"✅ Message sent from {senderName} to {receiverId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error sending message: {ex.Message}");
                throw new HubException($"Error sending message: {ex.Message}");
            }
        }
        private async Task<string> GetUserDisplayNameAsync(Guid userId, bool isAdmin)
        {
            if (isAdmin)
            {
                return "Admin";
            }

            try
            {
                var user = await _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new { u.FirstName, u.LastName, u.Email })
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.FirstName))
                    {
                        return $"{user.FirstName} {user.LastName}".Trim();
                    }
                    else if (!string.IsNullOrEmpty(user.Email))
                    {
                        return user.Email.Split('@')[0]; // Use username part of email
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error fetching user name: {ex.Message}");
            }

            // Fallback to claims if database fails
            var firstName = Context.User?.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastName = Context.User?.FindFirst(ClaimTypes.Surname)?.Value;
            var email = Context.User?.FindFirst(ClaimTypes.Email)?.Value;

            if (!string.IsNullOrEmpty(firstName))
            {
                return $"{firstName} {lastName}".Trim();
            }
            else if (!string.IsNullOrEmpty(email))
            {
                return email.Split('@')[0];
            }

            return "User"; // Ultimate fallback
        }

        // Optional: Add method for admin to send to multiple users
        public async Task SendBroadcastMessage(string content)
        {
            if (!Context.User.IsInRole("Admin"))
            {
                throw new HubException("Only admins can send broadcast messages");
            }

            var senderId = Guid.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var senderName = "Admin";

            var messageId = await _mediator.Send(new CreateChatMessageCommand
            {
                SenderId = senderId,
                ReceiverId = null, // Null indicates broadcast
                Content = content,
                IsFromAdmin = true
            });

            // Send to all connected users except sender
            await Clients.AllExcept(Context.ConnectionId).SendAsync("ReceiveMessage", new
            {
                Id = messageId.ToString(),
                SenderId = senderId.ToString(),
                SenderName = senderName,
                ReceiverId = (string?)null,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsFromAdmin = true
            });

            Console.WriteLine($"✅ Broadcast message sent by Admin");
        }

        // Optional: Add method to get user info
        public async Task<object> GetUserInfo(Guid userId)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new { u.FirstName, u.LastName, u.Email })
                .FirstOrDefaultAsync();

            return new
            {
                UserId = userId,
                DisplayName = user != null ?
                    (!string.IsNullOrEmpty(user.FirstName) ?
                        $"{user.FirstName} {user.LastName}".Trim() :
                        user.Email.Split('@')[0]) :
                    "User"
            };
        }
    }
}