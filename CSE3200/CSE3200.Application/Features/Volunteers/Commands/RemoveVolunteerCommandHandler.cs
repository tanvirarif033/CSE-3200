using CSE3200.Domain.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Volunteers.Commands
{
    public class RemoveVolunteerCommandHandler : IRequestHandler<RemoveVolunteerCommand, bool>
    {
        private readonly IVolunteerAssignmentService _volunteerService;

        public RemoveVolunteerCommandHandler(IVolunteerAssignmentService volunteerService)
        {
            _volunteerService = volunteerService;
        }

        public async Task<bool> Handle(RemoveVolunteerCommand request, CancellationToken cancellationToken)
        {
            _volunteerService.RemoveAssignment(request.AssignmentId);
            return await Task.FromResult(true);
        }
    }
}
