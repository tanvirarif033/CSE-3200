using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSE3200.Domain.Entities
{
    public class DisasterAlert : IEntity<Guid>
    {
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [Required, MaxLength(500)]
        public string Message { get; set; }

        [Required]
        public AlertSeverity Severity { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required]
        public int DisplayOrder { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        [NotMapped]
        public bool IsCurrent => IsActive &&
                               (!StartDate.HasValue || StartDate.Value <= DateTime.UtcNow) &&
                               (!EndDate.HasValue || EndDate.Value >= DateTime.UtcNow);
    }

    public enum AlertSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }
}