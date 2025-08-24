using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Domain.Entities
{
    public class Disaster : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime OccurredDate { get; set; }
        public DisasterSeverity Severity { get; set; }
        public int AffectedPeople { get; set; }
        public string RequiredAssistance { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty; // User ID
        public DateTime CreatedDate { get; set; }
        public DisasterStatus Status { get; set; }
        public string? ApprovedBy { get; set; } = null; // Nullable for pending approvals
        public DateTime? ApprovedDate { get; set; } = null; // Nullable for pending approvals
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