using CSE3200.Domain.Entities;
using MediatR;
using System;

namespace CSE3200.Application.Features.DisasterAlerts.Commands
{
    public class AddDisasterAlertCommand : IRequest<Guid>
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public AlertSeverity Severity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int DisplayOrder { get; set; }
        public string CreatedBy { get; set; }
    }
}