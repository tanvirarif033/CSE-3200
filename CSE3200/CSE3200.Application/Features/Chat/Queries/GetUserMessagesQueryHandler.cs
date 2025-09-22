using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSE3200.Infrastructure;

namespace CSE3200.Application.Features.Chat.Queries
{
    public class GetUserMessagesQueryHandler : IRequestHandler<GetUserMessagesQuery, List<ChatMessageDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetUserMessagesQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatMessageDto>> Handle(GetUserMessagesQuery request, CancellationToken cancellationToken)
        {
            // First, get the messages
            var messagesQuery = _context.ChatMessages.AsQueryable();

            if (request.OtherUserId.HasValue)
            {
                // Get conversation between two specific users
                messagesQuery = messagesQuery.Where(cm =>
                    (cm.SenderId == request.UserId && cm.ReceiverId == request.OtherUserId) ||
                    (cm.SenderId == request.OtherUserId && cm.ReceiverId == request.UserId));
            }
            else
            {
                // Get all messages for a user (both sent and received)
                messagesQuery = messagesQuery.Where(cm =>
                    cm.SenderId == request.UserId || cm.ReceiverId == request.UserId);
            }

            var messages = await messagesQuery
                .OrderBy(cm => cm.SentAt)
                .ToListAsync(cancellationToken);

            // Now get the user names for all senders
            var senderIds = messages.Select(m => m.SenderId).Distinct().ToList();
            var users = await _context.Users
                .Where(u => senderIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u, cancellationToken);

            // Map to DTOs with user names
            var messageDtos = messages.Select(cm =>
            {
                var user = users.ContainsKey(cm.SenderId) ? users[cm.SenderId] : null;
                var senderName = "User"; // Default fallback

                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.FirstName))
                    {
                        senderName = $"{user.FirstName} {user.LastName}".Trim();
                    }
                    else if (!string.IsNullOrEmpty(user.Email))
                    {
                        senderName = user.Email.Split('@')[0]; // Use email username
                    }
                }

                // For admin messages, use "Admin" as sender name
                if (cm.IsFromAdmin)
                {
                    senderName = "Admin";
                }

                return new ChatMessageDto
                {
                    Id = cm.Id,
                    SenderId = cm.SenderId,
                    SenderName = senderName,
                    ReceiverId = cm.ReceiverId,
                    Content = cm.Content,
                    SentAt = cm.SentAt,
                    IsRead = cm.IsRead,
                    IsFromAdmin = cm.IsFromAdmin
                };
            }).ToList();

            return messageDtos;
        }
    }
}