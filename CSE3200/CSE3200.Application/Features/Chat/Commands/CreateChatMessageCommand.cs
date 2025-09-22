using MediatR;
using System;

namespace CSE3200.Application.Features.Chat.Commands
{
    public class CreateChatMessageCommand : IRequest<Guid>
    {
        public Guid SenderId { get; set; }
        public Guid? ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsFromAdmin { get; set; }
    }
}