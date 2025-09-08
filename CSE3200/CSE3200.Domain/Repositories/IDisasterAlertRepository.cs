using CSE3200.Domain.Entities;
using System;
using System.Collections.Generic;

namespace CSE3200.Domain.Repositories
{
    public interface IDisasterAlertRepository : IRepository<DisasterAlert, Guid>
    {
        IList<DisasterAlert> GetActiveAlerts();
        IList<DisasterAlert> GetCurrentAlerts();
        IList<DisasterAlert> GetAlertsBySeverity(AlertSeverity severity);
        IList<DisasterAlert> GetAllAlertsWithPaging(int pageIndex, int pageSize, out int totalCount);
    }
}