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
            var query = _context.ChatMessages.AsQueryable();

            if (request.OtherUserId.HasValue)
            {
                // Get conversation between two specific users
                query = query.Where(cm =>
                    (cm.SenderId == request.UserId && cm.ReceiverId == request.OtherUserId) ||
                    (cm.SenderId == request.OtherUserId && cm.ReceiverId == request.UserId));
            }
            else
            {
                // Get all messages for a user (both sent and received)
                query = query.Where(cm =>
                    cm.SenderId == request.UserId || cm.ReceiverId == request.UserId);
            }

            var messages = await query
                .OrderBy(cm => cm.SentAt)
                .Select(cm => new ChatMessageDto
                {
                    Id = cm.Id,
                    SenderId = cm.SenderId,
                    SenderName = "User", // We'll fix this later when we add user navigation
                    ReceiverId = cm.ReceiverId,
                    Content = cm.Content,
                    SentAt = cm.SentAt,
                    IsRead = cm.IsRead,
                    IsFromAdmin = cm.IsFromAdmin
                })
                .ToListAsync(cancellationToken);

            return messages;
        }
    }
}