using CSE3200.Domain.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Disasters.Commands
{
    public class ApproveDisasterCommandHandler : IRequestHandler<ApproveDisasterCommand>
    {
        private readonly IDisasterService _disasterService;

        public ApproveDisasterCommandHandler(IDisasterService disasterService)
        {
            _disasterService = disasterService;
        }

        public async Task Handle(ApproveDisasterCommand request, CancellationToken cancellationToken)
        {
            _disasterService.ApproveDisaster(request.Id, request.ApprovedBy);
        }
    }
}
