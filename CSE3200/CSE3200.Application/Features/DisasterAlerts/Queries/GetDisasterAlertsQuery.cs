using CSE3200.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace CSE3200.Application.Features.DisasterAlerts.Queries
{
    public class GetDisasterAlertsQuery : IRequest<IList<DisasterAlert>>
    {
        public bool CurrentOnly { get; set; } = true;
    }
}