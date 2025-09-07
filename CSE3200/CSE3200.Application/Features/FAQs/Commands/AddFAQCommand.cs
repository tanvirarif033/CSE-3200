using CSE3200.Domain.Entities;
using MediatR;
using System;

namespace CSE3200.Application.Features.FAQs.Commands
{
    public class AddFAQCommand : IRequest<Guid>
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public FAQCategory Category { get; set; }
        public int DisplayOrder { get; set; }
        public string CreatedBy { get; set; }
    }
}