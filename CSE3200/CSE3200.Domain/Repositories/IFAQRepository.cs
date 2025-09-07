using CSE3200.Domain.Entities;
using System;
using System.Collections.Generic;

namespace CSE3200.Domain.Repositories
{
    public interface IFAQRepository : IRepository<FAQ, Guid>
    {
        IList<FAQ> GetActiveFAQs();
        IList<FAQ> GetFAQsByCategory(FAQCategory category);
        IList<FAQ> SearchFAQs(string searchTerm);
        IList<FAQ> GetAllFAQsWithPaging(int pageIndex, int pageSize, out int totalCount);
    }
}