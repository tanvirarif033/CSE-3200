using Microsoft.AspNetCore.Identity;
using System;

namespace CSE3200.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        //public string? PhoneNumber { get; set; }

    }
}
