using CSE3200.Domain;
using CSE3200.Infrastructure.Identity;
using MediatR;
using System.Collections.Generic;

namespace CSE3200.Application.Features.Users.Queries
{
    public class GetUsersListQuery : IRequest<(IList<ApplicationUser> data, int total, int totalDisplay)>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public DataTablesSearch Search { get; set; }
        public string RoleFilter { get; set; }
    }
}