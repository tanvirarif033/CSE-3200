using CSE3200.Infrastructure.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Users.Queries
{
    public class GetPendingVolunteerRequestsQuery : IRequest<List<ApplicationUser>>
    {
    }
}
