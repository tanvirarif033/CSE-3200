using CSE3200.Domain;
using CSE3200.Infrastructure;
using CSE3200.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Users.Queries
{
    public class GetUsersListQueryHandler : IRequestHandler<GetUsersListQuery, (IList<ApplicationUser> data, int total, int totalDisplay)>
    {
        private readonly ApplicationDbContext _context;

        public GetUsersListQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IList<ApplicationUser> data, int total, int totalDisplay)> Handle(
            GetUsersListQuery request, CancellationToken cancellationToken)
        {
            // Start with base user query - Remove the Include for UserRoles
            IQueryable<ApplicationUser> baseQuery = _context.Users.AsQueryable();

            // Apply role filter if specified
            if (!string.IsNullOrEmpty(request.RoleFilter))
            {
                // Get users in specific role via join
                baseQuery = from user in baseQuery
                            join userRole in _context.UserRoles on user.Id equals userRole.UserId
                            join role in _context.Roles on userRole.RoleId equals role.Id
                            where role.Name == request.RoleFilter
                            select user;
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(request.Search.Value))
            {
                baseQuery = baseQuery.Where(u =>
                    u.FirstName.Contains(request.Search.Value) ||
                    u.LastName.Contains(request.Search.Value) ||
                    u.Email.Contains(request.Search.Value) ||
                    u.UserName.Contains(request.Search.Value));
            }

            // Get distinct users after joins
            var distinctQuery = baseQuery.Distinct();

            // Get total count before pagination
            var total = await distinctQuery.CountAsync(cancellationToken);

            // Apply ordering
            IOrderedQueryable<ApplicationUser> orderedQuery;
            if (!string.IsNullOrEmpty(request.Order))
            {
                orderedQuery = request.Order switch
                {
                    "FirstName" => distinctQuery.OrderBy(u => u.FirstName),
                    "FirstName DESC" => distinctQuery.OrderByDescending(u => u.FirstName),
                    "LastName" => distinctQuery.OrderBy(u => u.LastName),
                    "LastName DESC" => distinctQuery.OrderByDescending(u => u.LastName),
                    "Email" => distinctQuery.OrderBy(u => u.Email),
                    "Email DESC" => distinctQuery.OrderByDescending(u => u.Email),
                    "UserName" => distinctQuery.OrderBy(u => u.UserName),
                    "UserName DESC" => distinctQuery.OrderByDescending(u => u.UserName),
                    "DateOfBirth" => distinctQuery.OrderBy(u => u.DateOfBirth),
                    "DateOfBirth DESC" => distinctQuery.OrderByDescending(u => u.DateOfBirth),
                    "RegistrationDate" => distinctQuery.OrderBy(u => u.RegistrationDate),
                    "RegistrationDate DESC" => distinctQuery.OrderByDescending(u => u.RegistrationDate),
                    _ => distinctQuery.OrderBy(u => u.FirstName)
                };
            }
            else
            {
                orderedQuery = distinctQuery.OrderBy(u => u.FirstName);
            }

            // Apply pagination
            var data = await orderedQuery
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return (data, total, total);
        }
    }
}