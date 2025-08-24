using CSE3200.Domain;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE3200.Application.Services
{
    public class DisasterService : IDisasterService
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly ILogger<DisasterService> _logger;

        public DisasterService(
            IApplicationUnitOfWork unitOfWork,
            ILogger<DisasterService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public void AddDisaster(Disaster disaster)
        {
            try
            {
                _logger.LogInformation("Adding disaster to repository: {Title}", disaster.Title);

                _unitOfWork.DisasterRepository.Add(disaster);
                _unitOfWork.Save();

                _logger.LogInformation("Disaster added successfully: {Id}", disaster.Id);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error while adding disaster");

                // Log inner exceptions for more details
                var innerEx = dbEx.InnerException;
                while (innerEx != null)
                {
                    _logger.LogError(innerEx, "Inner exception: {Message}", innerEx.Message);
                    innerEx = innerEx.InnerException;
                }

                throw new Exception($"Database error: {dbEx.Message}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding disaster");
                throw;
            }
        }

        public void UpdateDisaster(Disaster disaster)
        {
            _unitOfWork.DisasterRepository.Edit(disaster);
            _unitOfWork.Save();
        }

        public void DeleteDisaster(Guid id)
        {
            _unitOfWork.DisasterRepository.Remove(id);
            _unitOfWork.Save();
        }

        public Disaster GetDisaster(Guid id)
        {
            return _unitOfWork.DisasterRepository.GetById(id);
        }

        public (IList<Disaster> data, int total, int totalDisplay) GetDisasters(
            int pageIndex, int pageSize, string? order, DataTablesSearch search)
        {
            return _unitOfWork.DisasterRepository.GetPagedDisasters(
                pageIndex, pageSize, order, search);
        }

        public IList<Disaster> GetPendingApprovals()
        {
            return _unitOfWork.DisasterRepository.GetPendingApprovals();
        }

        public IList<Disaster> GetApprovedDisasters()
        {
            return _unitOfWork.DisasterRepository.GetApprovedDisasters();
        }

        public IList<Disaster> GetDisastersByUser(string userId)
        {
            return _unitOfWork.DisasterRepository.GetDisastersByUser(userId);
        }

        public void ApproveDisaster(Guid id, string approvedBy)
        {
            var disaster = GetDisaster(id);
            if (disaster != null && disaster.Status == DisasterStatus.PendingApproval)
            {
                disaster.Status = DisasterStatus.Approved;
                disaster.ApprovedBy = approvedBy;
                disaster.ApprovedDate = DateTime.UtcNow;
                UpdateDisaster(disaster);
            }
            else
            {
                throw new InvalidOperationException("Disaster not found or not in pending approval status");
            }
        }

        public void RejectDisaster(Guid id, string rejectedBy)
        {
            var disaster = GetDisaster(id);
            if (disaster != null && disaster.Status == DisasterStatus.PendingApproval)
            {
                disaster.Status = DisasterStatus.Rejected;
                disaster.ApprovedBy = rejectedBy;
                disaster.ApprovedDate = DateTime.UtcNow;
                UpdateDisaster(disaster);
            }
            else
            {
                throw new InvalidOperationException("Disaster not found or not in pending approval status");
            }
        }
    }
}