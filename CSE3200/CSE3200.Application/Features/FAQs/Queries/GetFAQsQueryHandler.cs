using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.FAQs.Queries
{
    public class GetFAQsQueryHandler : IRequestHandler<GetFAQsQuery, IList<FAQ>>
    {
        private readonly IFAQService _faqService;

        public GetFAQsQueryHandler(IFAQService faqService)
        {
            _faqService = faqService;
        }

        public async Task<IList<FAQ>> Handle(GetFAQsQuery request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                return _faqService.SearchFAQs(request.SearchTerm);
            }

            if (request.Category.HasValue)
            {
                return _faqService.GetFAQsByCategory(request.Category.Value);
            }

            return _faqService.GetActiveFAQs();
        }
    }
}