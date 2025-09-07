using CSE3200.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CSE3200.Web.Areas.Admin.Models
{
    public class AddFAQModel
    {
        [Required(ErrorMessage = "Question is required")]
        [MaxLength(200, ErrorMessage = "Question cannot exceed 200 characters")]
        public string Question { get; set; }

        [Required(ErrorMessage = "Answer is required")]
        public string Answer { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public FAQCategory Category { get; set; }

        [Required(ErrorMessage = "Display order is required")]
        [Range(0, 1000, ErrorMessage = "Display order must be between 0 and 1000")]
        public int DisplayOrder { get; set; } = 0;
    }
}