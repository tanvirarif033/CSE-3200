// Models/EnterOtpModel.cs
using System.ComponentModel.DataAnnotations;

namespace CSE3200.Web.Models
{
    public class EnterOtpModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "OTP Code")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "OTP must be 6 digits")]
        public string OTP { get; set; }
    }
}