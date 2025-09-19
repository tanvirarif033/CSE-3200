using System;
using System.ComponentModel.DataAnnotations;

namespace CSE3200.Domain.Entities
{
    public class Notification : IEntity<Guid>
    {
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        public NotificationType Type { get; set; }

        [Required]
        public bool IsRead { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public Guid? RelatedEntityId { get; set; } // For disaster ID

        [MaxLength(100)]
        public string? RelatedEntityType { get; set; } // "Disaster"

        [Required]
        public string UserId { get; set; } = string.Empty; // Target user ID
    }

    public enum NotificationType
    {
        DisasterCreated,
        DisasterApproved,
        General
    }
}