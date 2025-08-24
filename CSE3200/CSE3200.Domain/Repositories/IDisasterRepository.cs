using CSE3200.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Domain.Repositories
{
    public interface IDisasterRepository : IRepository<Disaster, Guid>
    {
        (IList<Disaster> data, int total, int totalDisplay) GetPagedDisasters(
            int pageIndex, int pageSize, string? order, DataTablesSearch search);

        IList<Disaster> GetPendingApprovals();
        IList<Disaster> GetApprovedDisasters();
        IList<Disaster> GetDisastersByUser(string userId);
    }
}