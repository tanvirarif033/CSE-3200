using CSE3200.Domain;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace CSE3200.Application.Services
{
    public class FAQService : IFAQService
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly ILogger<FAQService> _logger;

        public FAQService(
            IApplicationUnitOfWork unitOfWork,
            ILogger<FAQService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public void AddFAQ(FAQ faq)
        {
            try
            {
                _logger.LogInformation("Adding FAQ: {Question}", faq.Question);

                // Validate entity before saving - ADD THIS SECTION
                var validationContext = new ValidationContext(faq);
                var validationResults = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(faq, validationContext, validationResults, true);

                if (!isValid)
                {
                    foreach (var validationResult in validationResults)
                    {
                        _logger.LogError("Validation Error: {ErrorMessage}", validationResult.ErrorMessage);
                    }
                    throw new ValidationException("FAQ entity validation failed");
                }
                // END OF ADDED SECTION

                _unitOfWork.FAQRepository.Add(faq);
                _unitOfWork.Save();
                _logger.LogInformation("FAQ added successfully: {Id}", faq.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding FAQ");

                // Enhanced error logging - ADD THIS SECTION
                _logger.LogError("FAQ Entity Details:");
                _logger.LogError("Id: {Id}", faq.Id);
                _logger.LogError("Question: {Question}", faq.Question);
                _logger.LogError("Answer Length: {AnswerLength}", faq.Answer?.Length);
                _logger.LogError("Category: {Category}", faq.Category);
                _logger.LogError("DisplayOrder: {DisplayOrder}", faq.DisplayOrder);
                _logger.LogError("IsActive: {IsActive}", faq.IsActive);
                _logger.LogError("CreatedDate: {CreatedDate}", faq.CreatedDate);
                _logger.LogError("CreatedBy: {CreatedBy}", faq.CreatedBy);

                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner exception: {InnerMessage}", ex.InnerException.Message);

                    if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
                    {
                        _logger.LogError("SQL Error Number: {ErrorNumber}", sqlEx.Number);
                        _logger.LogError("SQL Error: {ErrorMessage}", sqlEx.Message);
                        _logger.LogError("SQL Procedure: {Procedure}", sqlEx.Procedure);
                        _logger.LogError("SQL Line Number: {LineNumber}", sqlEx.LineNumber);
                    }
                    else if (ex.InnerException is Microsoft.EntityFrameworkCore.DbUpdateException dbUpdateEx)
                    {
                        _logger.LogError("DbUpdate Exception: {Message}", dbUpdateEx.Message);
                    }
                }
                // END OF ADDED SECTION

                throw;
            }
        }

        public void UpdateFAQ(FAQ faq)
        {
            try
            {
                _logger.LogInformation("Updating FAQ: {Id}", faq.Id);
                _unitOfWork.FAQRepository.Edit(faq);
                _unitOfWork.Save();
                _logger.LogInformation("FAQ updated successfully: {Id}", faq.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating FAQ");
                throw;
            }
        }

        public void DeleteFAQ(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting FAQ: {Id}", id);
                _unitOfWork.FAQRepository.Remove(id);
                _unitOfWork.Save();
                _logger.LogInformation("FAQ deleted successfully: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting FAQ");
                throw;
            }
        }

        public FAQ GetFAQ(Guid id)
        {
            return _unitOfWork.FAQRepository.GetById(id);
        }

        public IList<FAQ> GetActiveFAQs()
        {
            return _unitOfWork.FAQRepository.GetActiveFAQs();
        }

        public IList<FAQ> GetFAQsByCategory(FAQCategory category)
        {
            return _unitOfWork.FAQRepository.GetFAQsByCategory(category);
        }

        public IList<FAQ> SearchFAQs(string searchTerm)
        {
            return _unitOfWork.FAQRepository.SearchFAQs(searchTerm);
        }

        public IList<FAQ> GetAllFAQsWithPaging(int pageIndex, int pageSize, out int totalCount)
        {
            return _unitOfWork.FAQRepository.GetAllFAQsWithPaging(pageIndex, pageSize, out totalCount);
        }

        public void ToggleFAQStatus(Guid id, string modifiedBy)
        {
            var faq = GetFAQ(id);
            if (faq != null)
            {
                faq.IsActive = !faq.IsActive;
                faq.ModifiedDate = DateTime.UtcNow;
                faq.ModifiedBy = modifiedBy;
                UpdateFAQ(faq);
            }
        }

    }
}