using MediatR;

namespace CSE3200.Application.Features.Users.Commands
{
    public class AddUserCommand : IRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime DateOfBirth { get; set; }
      //  public string? PhoneNumber { get; set; }
    }
}