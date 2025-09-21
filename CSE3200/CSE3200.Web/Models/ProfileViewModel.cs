// ProfileViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace CSE3200.Web.Models
{
    public class ProfileViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;

        [Display(Name = "State")]
        public string State { get; set; } = string.Empty;

        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; } = string.Empty;

        [Display(Name = "Emergency Contact Name")]
        public string EmergencyContactName { get; set; } = string.Empty;

        [Display(Name = "Emergency Contact Phone")]
        public string EmergencyContactPhone { get; set; } = string.Empty;

        [Display(Name = "Skills/Qualifications")]
        public string Skills { get; set; } = string.Empty;

        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }

        public string ProfilePictureUrl { get; set; } = string.Empty;
        public bool IsVolunteerRequested { get; set; }
        public string VolunteerRequestStatus { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
    }

    public class VolunteerRequestModel
    {
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Skills/Qualifications")]
        public string Skills { get; set; } = string.Empty;

        [Display(Name = "Emergency Contact Name")]
        public string EmergencyContactName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Emergency Contact Phone")]
        public string EmergencyContactPhone { get; set; } = string.Empty;
    }
}