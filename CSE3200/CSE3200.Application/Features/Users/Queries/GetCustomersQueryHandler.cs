using CSE3200.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Users.Queries
{
    public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, List<CustomerDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetCustomersQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            var customers = await _userManager.GetUsersInRoleAsync("Customer");
            return customers.Select(u => new CustomerDto
            {
                Email = u.Email,
                FullName = $"{u.FirstName} {u.LastName}"
            }).ToList();
        }
    }
}