using CSE3200.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace CSE3200.Application.Features.FAQs.Queries
{
    public class GetFAQsQuery : IRequest<IList<FAQ>>
    {
        public FAQCategory? Category { get; set; }
        public string SearchTerm { get; set; }
    }
}