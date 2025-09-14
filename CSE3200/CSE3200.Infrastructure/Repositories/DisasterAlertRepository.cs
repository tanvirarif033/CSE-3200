using CSE3200.Domain.Entities;
using CSE3200.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSE3200.Infrastructure.Repositories
{
    public class DisasterAlertRepository : Repository<DisasterAlert, Guid>, IDisasterAlertRepository
    {
        public DisasterAlertRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public IList<DisasterAlert> GetActiveAlerts()
        {
            return GetDynamic(
                filter: x => x.IsActive,
                orderBy: "DisplayOrder ASC, CreatedDate DESC",
                include: null,
                isTrackingOff: false
            );
        }

        public IList<DisasterAlert> GetCurrentAlerts()
        {
            var now = DateTime.UtcNow;
            return GetDynamic(
                filter: x => x.IsActive &&
                           (!x.StartDate.HasValue || x.StartDate.Value <= now) &&
                           (!x.EndDate.HasValue || x.EndDate.Value >= now),
                orderBy: "DisplayOrder ASC, CreatedDate DESC",
                include: null,
                isTrackingOff: false
            );
        }

        public IList<DisasterAlert> GetAlertsBySeverity(AlertSeverity severity)
        {
            return GetDynamic(
                filter: x => x.IsActive && x.Severity == severity,
                orderBy: "DisplayOrder ASC, CreatedDate DESC",
                include: null,
                isTrackingOff: false
            );
        }

        public IList<DisasterAlert> GetAllAlertsWithPaging(int pageIndex, int pageSize, out int totalCount)
        {
            var result = GetDynamic(
                filter: null,
                orderBy: "DisplayOrder ASC, CreatedDate DESC",
                include: null,
                pageIndex: pageIndex,
                pageSize: pageSize,
                isTrackingOff: false
            );

            totalCount = result.total;
            return result.data;
        }
    }
}