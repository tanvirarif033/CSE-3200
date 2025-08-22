using MediatR;
using System;

namespace CSE3200.Application.Features.Users.Commands
{
    public class UpdateUserCommand : IRequest
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
    }
}