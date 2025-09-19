using CSE3200.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CSE3200.Application.Features.Users.Queries
{
    public class GetPendingVolunteerRequestsQueryHandler : IRequestHandler<GetPendingVolunteerRequestsQuery, List<ApplicationUser>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetPendingVolunteerRequestsQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<ApplicationUser>> Handle(GetPendingVolunteerRequestsQuery request, CancellationToken cancellationToken)
        {
            var users = await _userManager.Users
                .Where(u => u.IsVolunteerRequested && u.VolunteerRequestStatus == "Pending")
                .ToListAsync(cancellationToken);

            return users;
        }
    }
}
