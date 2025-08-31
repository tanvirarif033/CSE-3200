using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Volunteers.Queries
{
    public class GetAllVolunteerAssignmentsQueryHandler : IRequestHandler<GetAllVolunteerAssignmentsQuery, IList<VolunteerAssignment>>
    {
        private readonly IVolunteerAssignmentService _volunteerService;

        public GetAllVolunteerAssignmentsQueryHandler(IVolunteerAssignmentService volunteerService)
        {
            _volunteerService = volunteerService;
        }

        public async Task<IList<VolunteerAssignment>> Handle(GetAllVolunteerAssignmentsQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_volunteerService.GetAllAssignments());
        }
    }
}