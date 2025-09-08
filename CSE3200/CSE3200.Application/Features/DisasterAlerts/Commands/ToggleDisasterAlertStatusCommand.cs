using MediatR;
using System;

namespace CSE3200.Application.Features.DisasterAlerts.Commands
{
    public class ToggleDisasterAlertStatusCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string ModifiedBy { get; set; }
    }
}