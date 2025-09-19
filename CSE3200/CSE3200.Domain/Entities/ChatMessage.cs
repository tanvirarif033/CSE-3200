using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSE3200.Domain.Entities
{
    public class ChatMessage : IEntity<Guid>  // Changed to IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid? ReceiverId { get; set; } // Null for admin broadcasts
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
        public bool IsFromAdmin { get; set; }

        // Remove the navigation properties for now - we'll add them back later
        // [ForeignKey("SenderId")]
        // public virtual ApplicationUser Sender { get; set; }

        // [ForeignKey("ReceiverId")]
        // public virtual ApplicationUser Receiver { get; set; }
    }
}