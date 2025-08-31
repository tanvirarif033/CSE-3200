using CSE3200.Domain.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Volunteers.Queries
{
    public class GetDisasterVolunteerCountQuery : IRequest<int>
    {
        public Guid DisasterId { get; set; }
    }

    public class GetDisasterVolunteerCountQueryHandler : IRequestHandler<GetDisasterVolunteerCountQuery, int>
    {
        private readonly IVolunteerAssignmentService _volunteerService;

        public GetDisasterVolunteerCountQueryHandler(IVolunteerAssignmentService volunteerService)
        {
            _volunteerService = volunteerService;
        }

        public async Task<int> Handle(GetDisasterVolunteerCountQuery request, CancellationToken cancellationToken)
        {
            var assignments = _volunteerService.GetDisasterAssignments(request.DisasterId);
            return assignments.Count;
        }
    }
}