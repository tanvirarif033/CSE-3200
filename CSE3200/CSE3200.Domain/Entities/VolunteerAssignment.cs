using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Domain.Entities
{
    public class VolunteerAssignment : IEntity<Guid>
    {
        public Guid Id { get; set; }

        [Required]
        public Guid DisasterId { get; set; }
        public Disaster Disaster { get; set; }

        [Required]
        public string VolunteerUserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string TaskDescription { get; set; } = string.Empty;

        public DateTime AssignedDate { get; set; }
        public string AssignedBy { get; set; } = string.Empty; // Admin user ID

        [MaxLength(20)]
        public string Status { get; set; } = "Assigned"; // Assigned, Completed, Cancelled

        // Add new fields for better tracking
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
    }
}