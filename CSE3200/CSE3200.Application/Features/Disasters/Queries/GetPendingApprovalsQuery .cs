using CSE3200.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Disasters.Queries
{
    public class GetPendingApprovalsQuery : IRequest<IList<Disaster>>
    {
    }
}
