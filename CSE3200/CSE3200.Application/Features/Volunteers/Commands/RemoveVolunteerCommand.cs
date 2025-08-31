using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Volunteers.Commands
{
    public class RemoveVolunteerCommand : IRequest<bool>
    {
        public Guid AssignmentId { get; set; }
    }
}
