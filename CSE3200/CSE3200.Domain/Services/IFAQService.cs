using CSE3200.Domain.Entities;
using System;
using System.Collections.Generic;

namespace CSE3200.Domain.Services
{
    public interface IFAQService
    {
        void AddFAQ(FAQ faq);
        void UpdateFAQ(FAQ faq);
        void DeleteFAQ(Guid id);
        FAQ GetFAQ(Guid id);
        IList<FAQ> GetActiveFAQs();
        IList<FAQ> GetFAQsByCategory(FAQCategory category);
        IList<FAQ> SearchFAQs(string searchTerm);
        IList<FAQ> GetAllFAQsWithPaging(int pageIndex, int pageSize, out int totalCount);
        void ToggleFAQStatus(Guid id, string modifiedBy);
    }
}