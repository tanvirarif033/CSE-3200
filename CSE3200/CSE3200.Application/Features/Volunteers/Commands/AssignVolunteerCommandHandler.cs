using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Volunteers.Commands
{
    public class AssignVolunteerCommandHandler : IRequestHandler<AssignVolunteerCommand, Guid>
    {
        private readonly IVolunteerAssignmentService _volunteerService;
        private readonly ILogger<AssignVolunteerCommandHandler> _logger;

        public AssignVolunteerCommandHandler(
            IVolunteerAssignmentService volunteerService,
            ILogger<AssignVolunteerCommandHandler> logger)
        {
            _volunteerService = volunteerService;
            _logger = logger;
        }

        public async Task<Guid> Handle(AssignVolunteerCommand request, CancellationToken cancellationToken)
        {
            var assignment = new VolunteerAssignment
            {
                Id = Guid.NewGuid(),
                DisasterId = request.DisasterId,
                VolunteerUserId = request.VolunteerUserId,
                TaskDescription = request.TaskDescription,
                AssignedBy = request.AssignedBy,
                AssignedDate = DateTime.UtcNow,
                Status = "Assigned"
            };

            _volunteerService.AssignVolunteer(assignment);
            return assignment.Id;
        }
    }
}