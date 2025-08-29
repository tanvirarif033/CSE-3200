using System;
using System.ComponentModel.DataAnnotations;

namespace CSE3200.Web.Areas.Admin.Models
{
    public class DonationModel
    {
        public Guid DisasterId { get; set; }
        public string DisasterTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        public string DonorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string DonorEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        public string DonorPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, 1000000, ErrorMessage = "Amount must be at least 1")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        public string PaymentMethod { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }


}