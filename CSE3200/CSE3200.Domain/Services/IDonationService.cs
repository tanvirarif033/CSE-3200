using CSE3200.Domain.Entities;
using System;
using System.Collections.Generic;

namespace CSE3200.Domain.Services
{
    public interface IDonationService
    {
        void AddDonation(Donation donation);
        Donation GetDonation(Guid id);
        (IList<Donation> data, int total, int totalDisplay) GetDonations(
            int pageIndex, int pageSize, string? order, DataTablesSearch search);

        IList<Donation> GetDonationsByUser(string userId);
        IList<Donation> GetDonationsByDisaster(Guid disasterId);
        decimal GetTotalDonationsByDisaster(Guid disasterId);
        decimal GetTotalDonationsByUser(string userId);
        IList<Donation> GetRecentDonations(int count);
        void UpdatePaymentStatus(Guid donationId, string transactionId, string status);
    }
}