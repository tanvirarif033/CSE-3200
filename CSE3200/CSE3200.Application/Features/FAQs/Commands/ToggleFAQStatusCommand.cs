using MediatR;
using System;

namespace CSE3200.Application.Features.FAQs.Commands
{
    public class ToggleFAQStatusCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string ModifiedBy { get; set; }
    }
}