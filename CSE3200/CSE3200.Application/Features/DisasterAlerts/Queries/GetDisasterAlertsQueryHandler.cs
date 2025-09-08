using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.DisasterAlerts.Queries
{
    public class GetDisasterAlertsQueryHandler : IRequestHandler<GetDisasterAlertsQuery, IList<DisasterAlert>>
    {
        private readonly IDisasterAlertService _alertService;

        public GetDisasterAlertsQueryHandler(IDisasterAlertService alertService)
        {
            _alertService = alertService;
        }

        public async Task<IList<DisasterAlert>> Handle(GetDisasterAlertsQuery request, CancellationToken cancellationToken)
        {
            if (request.CurrentOnly)
            {
                return _alertService.GetCurrentAlerts();
            }

            return _alertService.GetActiveAlerts();
        }
    }
}