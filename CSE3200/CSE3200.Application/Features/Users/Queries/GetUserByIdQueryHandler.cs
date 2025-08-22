using CSE3200.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Users.Queries
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            return await _userManager.FindByIdAsync(request.Id.ToString());
        }
    }
}