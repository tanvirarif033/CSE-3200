using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSE3200.Domain.Entities
{
    public class Disaster : IEntity<Guid>   
    {
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required, MaxLength(100)]
        public string Location { get; set; }

        [Required]
        public DisasterSeverity Severity { get; set; }

        [Required]
        public DisasterStatus Status { get; set; }

        [Required]
        public DateTime OccurredDate { get; set; }

        [Required]
        public int AffectedPeople { get; set; }

        [Required]
        public string RequiredAssistance { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        [NotMapped]
        public int VolunteerCount { get; set; }
    }

    public enum DisasterSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum DisasterStatus
    {
        PendingApproval,
        Approved,
        Rejected
    }
}