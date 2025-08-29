using CSE3200.Domain;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using System;
using System.Collections.Generic;

namespace CSE3200.Application.Services
{
    public class DonationService : IDonationService
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public DonationService(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void AddDonation(Donation donation)
        {
            _unitOfWork.DonationRepository.Add(donation);
            _unitOfWork.Save();
        }

        public Donation GetDonation(Guid id)
        {
            return _unitOfWork.DonationRepository.GetById(id);
        }

        public IList<Donation> GetDonationsByUser(string userId)
        {
            return _unitOfWork.DonationRepository.GetDonationsByUser(userId);
        }

        public (IList<Donation> data, int total, int totalDisplay) GetDonations(
            int pageIndex, int pageSize, string? order, DataTablesSearch search)
        {
            return _unitOfWork.DonationRepository.GetPagedDonations(
                pageIndex, pageSize, order, search);
        }

        public IList<Donation> GetDonationsByDisaster(Guid disasterId)
        {
            return _unitOfWork.DonationRepository.GetDonationsByDisaster(disasterId);
        }

        public decimal GetTotalDonationsByDisaster(Guid disasterId)
        {
            return _unitOfWork.DonationRepository.GetTotalDonationsByDisaster(disasterId);
        }

        public decimal GetTotalDonationsByUser(string userId)
        {
            return _unitOfWork.DonationRepository.GetTotalDonationsByUser(userId);
        }

        public IList<Donation> GetRecentDonations(int count)
        {
            return _unitOfWork.DonationRepository.GetRecentDonations(count);
        }

        public void UpdatePaymentStatus(Guid donationId, string transactionId, string status)
        {
            var donation = GetDonation(donationId);
            if (donation != null)
            {
                donation.TransactionId = transactionId;
                donation.PaymentStatus = status;
                _unitOfWork.DonationRepository.Edit(donation);
                _unitOfWork.Save();
            }
        }
    }
}