using CSE3200.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.FAQs.Commands
{
    public class UpdateFAQCommandHandler : IRequestHandler<UpdateFAQCommand, bool>
    {
        private readonly IFAQService _faqService;
        private readonly ILogger<UpdateFAQCommandHandler> _logger;

        public UpdateFAQCommandHandler(
            IFAQService faqService,
            ILogger<UpdateFAQCommandHandler> logger)
        {
            _faqService = faqService;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateFAQCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating FAQ with ID: {Id}", request.Id);

                var faq = _faqService.GetFAQ(request.Id);
                if (faq == null)
                {
                    _logger.LogWarning("FAQ not found with ID: {Id}", request.Id);
                    return false;
                }

                faq.Question = request.Question;
                faq.Answer = request.Answer;
                faq.Category = request.Category;
                faq.DisplayOrder = request.DisplayOrder;
                faq.ModifiedDate = DateTime.UtcNow;
                faq.ModifiedBy = request.ModifiedBy;

                _faqService.UpdateFAQ(faq);

                _logger.LogInformation("FAQ updated successfully with ID: {Id}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateFAQCommandHandler");
                throw;
            }
        }
    }
}