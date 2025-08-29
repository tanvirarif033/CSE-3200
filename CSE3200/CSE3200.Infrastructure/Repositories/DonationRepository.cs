using CSE3200.Domain;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CSE3200.Infrastructure.Repositories
{
    public class DonationRepository : Repository<Donation, Guid>, IDonationRepository
    {
        public DonationRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public (IList<Donation> data, int total, int totalDisplay) GetPagedDonations(
            int pageIndex, int pageSize, string? order, DataTablesSearch search)
        {
            if (string.IsNullOrWhiteSpace(search.Value))
                return GetDynamic(null, order, null, pageIndex, pageSize, true);
            else
                return GetDynamic(
                    x => x.DonorName.Contains(search.Value) ||
                         x.DonorEmail.Contains(search.Value) ||
                         x.DonorPhone.Contains(search.Value) ||
                         x.TransactionId.Contains(search.Value),
                    order, null, pageIndex, pageSize, true);
        }
        public IList<Donation> GetDonationsByUser(string userId)
        {
            return GetDynamic(
                filter: x => x.DonorUserId == userId,
                orderBy: "DonationDate DESC",
                include: null,
                isTrackingOff: false
            );
        }

        public IList<Donation> GetDonationsByDisaster(Guid disasterId)
        {
            return GetDynamic(
                filter: x => x.DisasterId == disasterId,
                orderBy: "DonationDate DESC",
                include: null,
                isTrackingOff: false
            );
        }

        public decimal GetTotalDonationsByDisaster(Guid disasterId)
        {
            var donations = GetDynamic(
                filter: x => x.DisasterId == disasterId && x.PaymentStatus == "Completed",
                orderBy: null,
                include: null,
                isTrackingOff: false
            );

            return donations.Sum(d => d.Amount);
        }

        public decimal GetTotalDonationsByUser(string userId)
        {
            var donations = GetDynamic(
                filter: x => x.DonorUserId == userId && x.PaymentStatus == "Completed",
                orderBy: null,
                include: null,
                isTrackingOff: false
            );

            return donations.Sum(d => d.Amount);
        }

        public IList<Donation> GetRecentDonations(int count)
        {
            return GetDynamic(
                filter: x => x.PaymentStatus == "Completed",
                orderBy: "DonationDate DESC",
                include: null,
                isTrackingOff: false
            ).Take(count).ToList();
        }
    }
}