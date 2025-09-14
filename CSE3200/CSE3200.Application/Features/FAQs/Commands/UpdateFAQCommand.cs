using CSE3200.Domain.Entities;
using MediatR;
using System;

namespace CSE3200.Application.Features.FAQs.Commands
{
    public class UpdateFAQCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public FAQCategory Category { get; set; }
        public int DisplayOrder { get; set; }
        public string ModifiedBy { get; set; }
    }
}