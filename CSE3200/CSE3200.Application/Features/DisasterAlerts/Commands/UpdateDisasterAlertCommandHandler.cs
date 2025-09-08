using CSE3200.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.DisasterAlerts.Commands
{
    public class UpdateDisasterAlertCommandHandler : IRequestHandler<UpdateDisasterAlertCommand, bool>
    {
        private readonly IDisasterAlertService _alertService;
        private readonly ILogger<UpdateDisasterAlertCommandHandler> _logger;

        public UpdateDisasterAlertCommandHandler(
            IDisasterAlertService alertService,
            ILogger<UpdateDisasterAlertCommandHandler> logger)
        {
            _alertService = alertService;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateDisasterAlertCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating disaster alert with ID: {Id}", request.Id);

                var alert = _alertService.GetAlert(request.Id);
                if (alert == null)
                {
                    _logger.LogWarning("Disaster alert not found with ID: {Id}", request.Id);
                    return false;
                }

                alert.Title = request.Title;
                alert.Message = request.Message;
                alert.Severity = request.Severity;
                alert.StartDate = request.StartDate;
                alert.EndDate = request.EndDate;
                alert.DisplayOrder = request.DisplayOrder;
                alert.ModifiedDate = DateTime.UtcNow;
                alert.ModifiedBy = request.ModifiedBy; // This should already be set

                _alertService.UpdateAlert(alert);

                _logger.LogInformation("Disaster alert updated successfully with ID: {Id}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateDisasterAlertCommandHandler");
                throw;
            }
        }
    }
}