using MediatR;
using System.Collections.Generic;

namespace CSE3200.Application.Features.Users.Queries
{
    public class GetCustomersQuery : IRequest<List<CustomerDto>>
    {
    }
}