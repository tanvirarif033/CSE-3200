using CSE3200.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.DisasterAlerts.Commands
{
    public class ToggleDisasterAlertStatusCommandHandler : IRequestHandler<ToggleDisasterAlertStatusCommand, bool>
    {
        private readonly IDisasterAlertService _alertService;
        private readonly ILogger<ToggleDisasterAlertStatusCommandHandler> _logger;

        public ToggleDisasterAlertStatusCommandHandler(
            IDisasterAlertService alertService,
            ILogger<ToggleDisasterAlertStatusCommandHandler> logger)
        {
            _alertService = alertService;
            _logger = logger;
        }

        public async Task<bool> Handle(ToggleDisasterAlertStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Toggling disaster alert status for ID: {Id}", request.Id);
                _alertService.ToggleAlertStatus(request.Id, request.ModifiedBy);
                _logger.LogInformation("Disaster alert status toggled successfully for ID: {Id}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ToggleDisasterAlertStatusCommandHandler");
                throw;
            }
        }
    }
}