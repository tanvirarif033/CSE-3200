using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using CSE3200.Domain.Entities;

namespace CSE3200.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime DateOfBirth { get; set; }

        // Navigation property for notifications
        public virtual ICollection<Notification> Notifications { get; set; }

        public ApplicationUser()
        {
            Notifications = new HashSet<Notification>();
        }
    }
}