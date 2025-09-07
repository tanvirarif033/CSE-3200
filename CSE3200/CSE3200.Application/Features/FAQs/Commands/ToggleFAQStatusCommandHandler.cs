using CSE3200.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.FAQs.Commands
{
    public class ToggleFAQStatusCommandHandler : IRequestHandler<ToggleFAQStatusCommand, bool>
    {
        private readonly IFAQService _faqService;
        private readonly ILogger<ToggleFAQStatusCommandHandler> _logger;

        public ToggleFAQStatusCommandHandler(
            IFAQService faqService,
            ILogger<ToggleFAQStatusCommandHandler> logger)
        {
            _faqService = faqService;
            _logger = logger;
        }

        public async Task<bool> Handle(ToggleFAQStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Toggling FAQ status for ID: {Id}", request.Id);
                _faqService.ToggleFAQStatus(request.Id, request.ModifiedBy);
                _logger.LogInformation("FAQ status toggled successfully for ID: {Id}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ToggleFAQStatusCommandHandler");
                throw;
            }
        }
    }
}