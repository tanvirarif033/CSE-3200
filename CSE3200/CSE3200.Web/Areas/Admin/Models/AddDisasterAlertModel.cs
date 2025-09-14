using CSE3200.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace CSE3200.Web.Areas.Admin.Models
{
    public class AddDisasterAlertModel
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Severity is required")]
        public AlertSeverity Severity { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Display order is required")]
        [Range(0, 1000, ErrorMessage = "Display order must be between 0 and 1000")]
        public int DisplayOrder { get; set; } = 0;
    }
}