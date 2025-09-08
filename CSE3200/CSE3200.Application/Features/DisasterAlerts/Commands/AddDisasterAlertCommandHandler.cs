using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.DisasterAlerts.Commands
{
    public class AddDisasterAlertCommandHandler : IRequestHandler<AddDisasterAlertCommand, Guid>
    {
        private readonly IDisasterAlertService _alertService;
        private readonly ILogger<AddDisasterAlertCommandHandler> _logger;

        public AddDisasterAlertCommandHandler(
            IDisasterAlertService alertService,
            ILogger<AddDisasterAlertCommandHandler> logger)
        {
            _alertService = alertService;
            _logger = logger;
        }

        public async Task<Guid> Handle(AddDisasterAlertCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating disaster alert: {Title}", request.Title);

                var alert = new DisasterAlert
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Message = request.Message,
                    Severity = request.Severity,
                    IsActive = true,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    DisplayOrder = request.DisplayOrder,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy,
                    // ADD THIS LINE - Set ModifiedBy to same as CreatedBy for new records
                    ModifiedBy = request.CreatedBy,
                    // ADD THIS LINE - Set ModifiedDate to same as CreatedDate for new records
                    ModifiedDate = DateTime.UtcNow
                };

                _alertService.AddAlert(alert);

                _logger.LogInformation("Disaster alert created successfully with ID: {Id}", alert.Id);
                return alert.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddDisasterAlertCommandHandler");
                throw;
            }
        }
    }
}