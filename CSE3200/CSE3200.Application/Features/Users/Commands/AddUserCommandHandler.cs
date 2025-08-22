using CSE3200.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Users.Commands
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;

        public AddUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = (IUserEmailStore<ApplicationUser>)_userStore;
        }

        public async Task Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                RegistrationDate = DateTime.UtcNow,
                //PhoneNumber = request.PhoneNumber
            };

            await _userStore.SetUserNameAsync(user, request.Email, cancellationToken);
            await _emailStore.SetEmailAsync(user, request.Email, cancellationToken);

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, request.Role);
            }
            else
            {
                // Handle errors if needed
                throw new Exception($"User creation failed: {string.Join(", ", result.Errors)}");
            }
        }
    }
}