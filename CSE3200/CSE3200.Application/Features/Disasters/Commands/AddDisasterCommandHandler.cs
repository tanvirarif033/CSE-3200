using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Disasters.Commands
{
    public class AddDisasterCommandHandler : IRequestHandler<AddDisasterCommand, Guid>
    {
        private readonly IDisasterService _disasterService;
        private readonly ILogger<AddDisasterCommandHandler> _logger;

        public AddDisasterCommandHandler(
            IDisasterService disasterService,
            ILogger<AddDisasterCommandHandler> logger)
        {
            _disasterService = disasterService;
            _logger = logger;
        }

        public async Task<Guid> Handle(AddDisasterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating disaster for user: {User}, IsAdmin: {IsAdmin}",
                    request.CreatedBy, request.IsAdmin);

                var disaster = new Disaster
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Description = request.Description,
                    Location = request.Location,
                    OccurredDate = request.OccurredDate,
                    Severity = request.Severity,
                    AffectedPeople = request.AffectedPeople,
                    RequiredAssistance = request.RequiredAssistance,
                    CreatedBy = request.CreatedBy,
                    CreatedDate = DateTime.UtcNow,
                    Status = request.IsAdmin ? DisasterStatus.Approved : DisasterStatus.PendingApproval,
                    ApprovedBy = request.IsAdmin ? request.CreatedBy : null,
                    ApprovedDate = request.IsAdmin ? DateTime.UtcNow : null
                };

                _logger.LogInformation("Disaster created with Status: {Status}", disaster.Status);

                _disasterService.AddDisaster(disaster);

                _logger.LogInformation("Disaster saved successfully with ID: {Id}", disaster.Id);

                return disaster.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddDisasterCommandHandler");
                throw;
            }
        }
    }
}