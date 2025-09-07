using CSE3200.Domain;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CSE3200.Infrastructure.Repositories
{
    public class FAQRepository : Repository<FAQ, Guid>, IFAQRepository
    {
        public FAQRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public IList<FAQ> GetActiveFAQs()
        {
            return GetDynamic(
                filter: x => x.IsActive,
                orderBy: "DisplayOrder ASC, CreatedDate DESC",
                include: null,
                isTrackingOff: false
            );
        }

        public IList<FAQ> GetFAQsByCategory(FAQCategory category)
        {
            return GetDynamic(
                filter: x => x.Category == category && x.IsActive,
                orderBy: "DisplayOrder ASC, CreatedDate DESC",
                include: null,
                isTrackingOff: false
            );
        }

        public IList<FAQ> SearchFAQs(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetActiveFAQs();

            return GetDynamic(
                filter: x => x.IsActive &&
                           (x.Question.Contains(searchTerm) || x.Answer.Contains(searchTerm)),
                orderBy: "DisplayOrder ASC, CreatedDate DESC",
                include: null,
                isTrackingOff: false
            );
        }

        public IList<FAQ> GetAllFAQsWithPaging(int pageIndex, int pageSize, out int totalCount)
        {
            var result = GetDynamic(
                filter: null,
                orderBy: "DisplayOrder ASC, CreatedDate DESC",
                include: null,
                pageIndex: pageIndex,
                pageSize: pageSize,
                isTrackingOff: false
            );

            totalCount = result.total;
            return result.data;
        }
    }
}