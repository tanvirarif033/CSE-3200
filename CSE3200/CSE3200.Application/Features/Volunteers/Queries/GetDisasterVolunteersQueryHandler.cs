using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Volunteers.Queries
{
    public class GetDisasterVolunteersQueryHandler : IRequestHandler<GetDisasterVolunteersQuery, IList<VolunteerAssignment>>
    {
        private readonly IVolunteerAssignmentService _volunteerService;

        public GetDisasterVolunteersQueryHandler(IVolunteerAssignmentService volunteerService)
        {
            _volunteerService = volunteerService;
        }

        public async Task<IList<VolunteerAssignment>> Handle(GetDisasterVolunteersQuery request, CancellationToken cancellationToken)
        {
            return _volunteerService.GetDisasterAssignments(request.DisasterId);
        }
    }
}