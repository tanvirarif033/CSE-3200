using CSE3200.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Disasters.Commands
{
    public class AddDisasterCommand : IRequest<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime OccurredDate { get; set; }
        public DisasterSeverity Severity { get; set; }
        public int AffectedPeople { get; set; }
        public string RequiredAssistance { get; set; }
        public string CreatedBy { get; set; }
        public bool IsAdmin { get; set; }
    }
} 
