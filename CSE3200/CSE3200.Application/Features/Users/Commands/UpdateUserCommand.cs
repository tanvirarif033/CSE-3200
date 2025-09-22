using MediatR;
using System;

namespace CSE3200.Application.Features.Users.Commands
{
    // UpdateUserCommand.cs
    public class UpdateUserCommand : IRequest
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }

        // Add these new properties
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? Skills { get; set; }
    }
}