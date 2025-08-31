using CSE3200.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;

namespace CSE3200.Application.Features.Volunteers.Queries
{
    public class GetDisasterVolunteersQuery : IRequest<IList<VolunteerAssignment>>
    {
        public Guid DisasterId { get; set; }
    }
}