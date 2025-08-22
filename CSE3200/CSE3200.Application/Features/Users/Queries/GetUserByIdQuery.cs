using CSE3200.Infrastructure.Identity;
using MediatR;
using System;

namespace CSE3200.Application.Features.Users.Queries
{
    public class GetUserByIdQuery : IRequest<ApplicationUser>
    {
        public Guid Id { get; set; }
    }
}