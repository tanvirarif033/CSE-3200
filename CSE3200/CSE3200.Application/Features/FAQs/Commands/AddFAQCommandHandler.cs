using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.FAQs.Commands
{
    public class AddFAQCommandHandler : IRequestHandler<AddFAQCommand, Guid>
    {
        private readonly IFAQService _faqService;
        private readonly ILogger<AddFAQCommandHandler> _logger;

        public AddFAQCommandHandler(
            IFAQService faqService,
            ILogger<AddFAQCommandHandler> logger)
        {
            _faqService = faqService;
            _logger = logger;
        }

        public async Task<Guid> Handle(AddFAQCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Question))
                    throw new ArgumentException("Question is required");

                if (string.IsNullOrEmpty(request.Answer))
                    throw new ArgumentException("Answer is required");

                if (string.IsNullOrEmpty(request.CreatedBy))
                    throw new ArgumentException("CreatedBy is required");

                if (request.Question.Length > 200)
                    throw new ArgumentException("Question cannot exceed 200 characters");

                _logger.LogInformation("Creating FAQ: {Question}", request.Question);

                var faq = new FAQ
                {
                    Id = Guid.NewGuid(),
                    Question = request.Question.Trim(),
                    Answer = request.Answer.Trim(),
                    Category = request.Category,
                    DisplayOrder = request.DisplayOrder,
                    IsActive = true, // This is set to true by default
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy,
                    ModifiedBy = null, // EXPLICITLY SET TO NULL
                    ModifiedDate = null // EXPLICITLY SET TO NULL
                };

                _logger.LogInformation("Calling FAQService.AddFAQ");
                _faqService.AddFAQ(faq);

                _logger.LogInformation("FAQ created successfully with ID: {Id}", faq.Id);
                return faq.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddFAQCommandHandler");
                _logger.LogError("Error details: {Message}, StackTrace: {StackTrace}",
                    ex.Message, ex.StackTrace);
                throw;
            }
        }
    }
}