using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Disasters.Commands
{
    public class ApproveDisasterCommand : IRequest
    {
        public Guid Id { get; set; }
        public string ApprovedBy { get; set; }
    }
}
