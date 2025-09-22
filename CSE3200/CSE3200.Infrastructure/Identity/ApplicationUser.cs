using Microsoft.AspNetCore.Identity;
using System;

namespace CSE3200.Infrastructure.Identity
{
    // ApplicationUser.cs
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }

        // Add these new properties for profile functionality
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? Skills { get; set; }
        public bool IsVolunteerRequested { get; set; }
        public DateTime? VolunteerRequestDate { get; set; }
        public string? VolunteerRequestStatus { get; set; } // Pending, Approved, Rejected
        public string? ProfilePictureUrl { get; set; }
    }
}
