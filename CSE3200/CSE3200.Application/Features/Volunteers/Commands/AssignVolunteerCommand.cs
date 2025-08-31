using MediatR;
using System;

namespace CSE3200.Application.Features.Volunteers.Commands
{
    public class AssignVolunteerCommand : IRequest<Guid>
    {
        public Guid DisasterId { get; set; }
        public string VolunteerUserId { get; set; }
        public string TaskDescription { get; set; }
        public string AssignedBy { get; set; }
    }
}
