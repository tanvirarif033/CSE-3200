using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using MediatR;
using CSE3200.Application.Features.Chat.Commands;

namespace CSE3200.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
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

                // Get the sender's display name
                var senderName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ??
                                Context.User?.FindFirst(ClaimTypes.Email)?.Value ??
                                (isAdmin ? "Admin" : "User");

                var messageId = await _mediator.Send(new CreateChatMessageCommand
                {
                    SenderId = senderId,
                    ReceiverId = receiverGuid,
                    Content = content,
                    IsFromAdmin = isAdmin
                });

                // Send to receiver - INCLUDING SENDER NAME
                await Clients.Group(receiverId).SendAsync("ReceiveMessage", new
                {
                    Id = messageId.ToString(),
                    SenderId = senderId.ToString(),
                    SenderName = senderName, // ← ADD THIS
                    ReceiverId = receiverId,
                    Content = content,
                    SentAt = DateTime.UtcNow,
                    IsFromAdmin = isAdmin
                });

                Console.WriteLine("✅ Message sent to receiver successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SERVER ERROR: {ex.Message}");
                throw new HubException($"Server error: {ex.Message}");
            }
        }



    }
}
