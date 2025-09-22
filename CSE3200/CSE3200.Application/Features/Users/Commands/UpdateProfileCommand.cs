using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Users.Commands
{
    // UpdateProfileCommand.cs
    public class UpdateProfileCommand : IRequest
    {
        public Guid UserId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? Skills { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }

    // RequestVolunteerCommand.cs
    public class RequestVolunteerCommand : IRequest
    {
        public Guid UserId { get; set; }
    }

    // UpdateVolunteerStatusCommand.cs
    public class UpdateVolunteerStatusCommand : IRequest
    {
        public Guid UserId { get; set; }
        public string Status { get; set; } // Approved, Rejected
        public string? Reason { get; set; }
    }
}
