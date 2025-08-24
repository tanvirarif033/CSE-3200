using CSE3200.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CSE3200.Web.Areas.Admin.Models
{
    public class EditDisasterModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public DateTime OccurredDate { get; set; }

        [Required]
        public DisasterSeverity Severity { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int AffectedPeople { get; set; }

        [Required]
        public string RequiredAssistance { get; set; }
    }
}
