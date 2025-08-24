using CSE3200.Domain;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CSE3200.Infrastructure.Repositories
{
    public class DisasterRepository : Repository<Disaster, Guid>, IDisasterRepository
    {
        public DisasterRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public (IList<Disaster> data, int total, int totalDisplay) GetPagedDisasters(
            int pageIndex, int pageSize, string? order, DataTablesSearch search)
        {
            if (string.IsNullOrWhiteSpace(search.Value))
                return GetDynamic(null, order, null, pageIndex, pageSize, true);
            else
                return GetDynamic(
                    x => x.Title.Contains(search.Value) ||
                         x.Location.Contains(search.Value) ||
                         x.Description.Contains(search.Value) ||
                         x.RequiredAssistance.Contains(search.Value),
                    order, null, pageIndex, pageSize, true);
        }

        public IList<Disaster> GetPendingApprovals()
        {
            return GetDynamic(
                filter: x => x.Status == DisasterStatus.PendingApproval,
                orderBy: "CreatedDate DESC",
                include: null,
                isTrackingOff: false
            );
        }

        public IList<Disaster> GetApprovedDisasters()
        {
            return GetDynamic(
                filter: x => x.Status == DisasterStatus.Approved,
                orderBy: "OccurredDate DESC",
                include: null,
                isTrackingOff: false
            );
        }

        public IList<Disaster> GetDisastersByUser(string userId)
        {
            return GetDynamic(
                filter: x => x.CreatedBy == userId,
                orderBy: "CreatedDate DESC",
                include: null,
                isTrackingOff: false
            );
        }
    }
}