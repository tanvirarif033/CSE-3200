using CSE3200.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CSE3200.Web.Areas.Admin.Models
{
    public class AddDisasterModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Occurred date is required")]
        public DateTime OccurredDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Severity level is required")]
        public DisasterSeverity Severity { get; set; }

        [Required(ErrorMessage = "Number of affected people is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Must affect at least 1 person")]
        public int AffectedPeople { get; set; } = 1;

        [Required(ErrorMessage = "Required assistance is required")]
        public string RequiredAssistance { get; set; } = string.Empty;
    }
}