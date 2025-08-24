using CSE3200.Domain.Entities;

namespace CSE3200.Domain.Services
{
    public interface IDisasterService
    {
        void AddDisaster(Disaster disaster);
        void UpdateDisaster(Disaster disaster);
        void DeleteDisaster(Guid id);
        Disaster GetDisaster(Guid id);
        (IList<Disaster> data, int total, int totalDisplay) GetDisasters(
            int pageIndex, int pageSize, string? order, DataTablesSearch search);

        IList<Disaster> GetPendingApprovals();
        IList<Disaster> GetApprovedDisasters();
        IList<Disaster> GetDisastersByUser(string userId);
        void ApproveDisaster(Guid id, string approvedBy);
        void RejectDisaster(Guid id, string rejectedBy);
    }
}