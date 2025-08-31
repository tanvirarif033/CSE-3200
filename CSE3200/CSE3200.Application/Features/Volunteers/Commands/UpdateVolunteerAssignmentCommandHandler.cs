using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Volunteers.Commands
{
    public class UpdateVolunteerAssignmentCommandHandler : IRequestHandler<UpdateVolunteerAssignmentCommand, bool>
    {
        private readonly IVolunteerAssignmentService _volunteerService;

        public UpdateVolunteerAssignmentCommandHandler(IVolunteerAssignmentService volunteerService)
        {
            _volunteerService = volunteerService;
        }

        public async Task<bool> Handle(UpdateVolunteerAssignmentCommand request, CancellationToken cancellationToken)
        {
            var assignment = _volunteerService.GetAssignmentById(request.Id);
            if (assignment == null)
                return false;

            assignment.TaskDescription = request.TaskDescription;
            assignment.Status = request.Status;

            _volunteerService.UpdateAssignment(assignment);
            return await Task.FromResult(true);
        }
    }
}