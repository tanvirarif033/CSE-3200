using CSE3200.Domain;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CSE3200.Application.Services
{
    public class DisasterAlertService : IDisasterAlertService
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly ILogger<DisasterAlertService> _logger;

        public DisasterAlertService(
            IApplicationUnitOfWork unitOfWork,
            ILogger<DisasterAlertService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public void AddAlert(DisasterAlert alert)
        {
            try
            {
                // Validate entity before saving
                var validationContext = new ValidationContext(alert);
                var validationResults = new List<ValidationResult>();

                if (!Validator.TryValidateObject(alert, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(vr => vr.ErrorMessage);
                    throw new ValidationException($"Validation failed: {string.Join(", ", errorMessages)}");
                }

                _logger.LogInformation("Adding disaster alert: {Title}", alert.Title);
                _unitOfWork.DisasterAlertRepository.Add(alert);
                _unitOfWork.Save();
                _logger.LogInformation("Disaster alert added successfully: {Id}", alert.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding disaster alert. Title: {Title}, CreatedBy: {CreatedBy}",
                    alert?.Title, alert?.CreatedBy);
                throw;
            }
        }
        public void UpdateAlert(DisasterAlert alert)
        {
            try
            {
                _logger.LogInformation("Updating disaster alert: {Id}", alert.Id);
                _unitOfWork.DisasterAlertRepository.Edit(alert);
                _unitOfWork.Save();
                _logger.LogInformation("Disaster alert updated successfully: {Id}", alert.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating disaster alert");
                throw;
            }
        }

        public void DeleteAlert(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting disaster alert: {Id}", id);
                _unitOfWork.DisasterAlertRepository.Remove(id);
                _unitOfWork.Save();
                _logger.LogInformation("Disaster alert deleted successfully: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting disaster alert");
                throw;
            }
        }

        public DisasterAlert GetAlert(Guid id)
        {
            return _unitOfWork.DisasterAlertRepository.GetById(id);
        }

        public IList<DisasterAlert> GetActiveAlerts()
        {
            return _unitOfWork.DisasterAlertRepository.GetActiveAlerts();
        }

        public IList<DisasterAlert> GetCurrentAlerts()
        {
            return _unitOfWork.DisasterAlertRepository.GetCurrentAlerts();
        }

        public IList<DisasterAlert> GetAlertsBySeverity(AlertSeverity severity)
        {
            return _unitOfWork.DisasterAlertRepository.GetAlertsBySeverity(severity);
        }

        public IList<DisasterAlert> GetAllAlertsWithPaging(int pageIndex, int pageSize, out int totalCount)
        {
            return _unitOfWork.DisasterAlertRepository.GetAllAlertsWithPaging(pageIndex, pageSize, out totalCount);
        }

        public void ToggleAlertStatus(Guid id, string modifiedBy)
        {
            var alert = GetAlert(id);
            if (alert != null)
            {
                alert.IsActive = !alert.IsActive;
                alert.ModifiedDate = DateTime.UtcNow;
                alert.ModifiedBy = modifiedBy;
                UpdateAlert(alert);
            }
        }
    }
}