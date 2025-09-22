using MediatR;
using Microsoft.EntityFrameworkCore;
using CSE3200.Domain.Entities;
using CSE3200.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Chat.Commands
{
    public class CreateChatMessageCommandHandler : IRequestHandler<CreateChatMessageCommand, Guid>
    {
        private readonly ApplicationDbContext _context;

        public CreateChatMessageCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateChatMessageCommand request, CancellationToken cancellationToken)
        {
            var message = new ChatMessage
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                IsFromAdmin = request.IsFromAdmin,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync(cancellationToken);

            return message.Id;
        }
    }
}