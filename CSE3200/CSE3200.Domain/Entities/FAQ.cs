using System;
using System.ComponentModel.DataAnnotations;

namespace CSE3200.Domain.Entities
{
    public class FAQ : IEntity<Guid>
    {
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }

        [Required]
        public FAQCategory Category { get; set; }

        [Required]
        public int DisplayOrder { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; } // CHANGE TO nullable string
    }

    public enum FAQCategory
    {
        General,
        DisasterReporting,
        Volunteer,
        Donation,
        Technical,
        Account
    }
}