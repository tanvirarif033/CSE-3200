using MediatR;
using System;
using System.Collections.Generic;

namespace CSE3200.Application.Features.Chat.Queries
{
    public class GetUserMessagesQuery : IRequest<List<ChatMessageDto>>
    {
        public Guid UserId { get; set; }
        public Guid? OtherUserId { get; set; } // For admin viewing specific user
    }

    public class ChatMessageDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public Guid? ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public bool IsFromAdmin { get; set; }
    }
}