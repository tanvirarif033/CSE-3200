using CSE3200.Domain.Entities;
using System;
using System.Collections.Generic;

namespace CSE3200.Domain.Services
{
    public interface IDisasterAlertService
    {
        void AddAlert(DisasterAlert alert);
        void UpdateAlert(DisasterAlert alert);
        void DeleteAlert(Guid id);
        DisasterAlert GetAlert(Guid id);
        IList<DisasterAlert> GetActiveAlerts();
        IList<DisasterAlert> GetCurrentAlerts();
        IList<DisasterAlert> GetAlertsBySeverity(AlertSeverity severity);
        IList<DisasterAlert> GetAllAlertsWithPaging(int pageIndex, int pageSize, out int totalCount);
        void ToggleAlertStatus(Guid id, string modifiedBy);
    }
}