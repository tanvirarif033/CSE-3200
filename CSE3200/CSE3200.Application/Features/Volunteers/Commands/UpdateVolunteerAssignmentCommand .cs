using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Volunteers.Commands
{
    public class UpdateVolunteerAssignmentCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string TaskDescription { get; set; } = string.Empty;
        public string Status { get; set; } = "Assigned";
    }
}
