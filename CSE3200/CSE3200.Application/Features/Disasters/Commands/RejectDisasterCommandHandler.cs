using CSE3200.Domain.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Disasters.Commands
{
    public class RejectDisasterCommandHandler : IRequestHandler<RejectDisasterCommand>
    {
        private readonly IDisasterService _disasterService;

        public RejectDisasterCommandHandler(IDisasterService disasterService)
        {
            _disasterService = disasterService;
        }

        public async Task Handle(RejectDisasterCommand request, CancellationToken cancellationToken)
        {
            _disasterService.RejectDisaster(request.Id, request.RejectedBy);
        }
    }
}
