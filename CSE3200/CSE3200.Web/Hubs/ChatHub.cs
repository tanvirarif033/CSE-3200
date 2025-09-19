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

        public async Task SendMessageToUser(Guid receiverId, string content)
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return;

            var senderId = Guid.Parse(userIdClaim.Value);
            var isAdmin = Context.User.IsInRole("Admin");

            // Save to database
            var messageId = await _mediator.Send(new CreateChatMessageCommand
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                IsFromAdmin = isAdmin
            });

            // Send to receiver
            await Clients.Group(receiverId.ToString()).SendAsync("ReceiveMessage", new
            {
                Id = messageId,
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsFromAdmin = isAdmin
            });

            // If admin sends message, also update admin UI
            if (isAdmin)
            {
                await Clients.Group("Admins").SendAsync("AdminMessageSent", new
                {
                    Id = messageId,
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = content,
                    SentAt = DateTime.UtcNow
                });
            }
        }
    }
}