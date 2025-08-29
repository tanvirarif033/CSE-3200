using CSE3200.Domain.Entities;
using System;
using System.Collections.Generic;

namespace CSE3200.Domain.Repositories
{
    public interface IDonationRepository : IRepository<Donation, Guid>
    {
        (IList<Donation> data, int total, int totalDisplay) GetPagedDonations(
            int pageIndex, int pageSize, string? order, DataTablesSearch search);

        IList<Donation> GetDonationsByUser(string userId);
        IList<Donation> GetDonationsByDisaster(Guid disasterId);
        decimal GetTotalDonationsByDisaster(Guid disasterId);
        decimal GetTotalDonationsByUser(string userId);
        IList<Donation> GetRecentDonations(int count);
    }
}