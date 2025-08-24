using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Disasters.Queries
{
    public class GetPendingApprovalsQueryHandler : IRequestHandler<GetPendingApprovalsQuery, IList<Disaster>>
    {
        private readonly IDisasterService _disasterService;

        public GetPendingApprovalsQueryHandler(IDisasterService disasterService)
        {
            _disasterService = disasterService;
        }

        public async Task<IList<Disaster>> Handle(GetPendingApprovalsQuery request, CancellationToken cancellationToken)
        {
            return _disasterService.GetPendingApprovals();
        }
    }
}
