using CSE3200.Domain.Entities;
using CSE3200.Domain.Repositories;
using CSE3200.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product, Guid>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        // Enhanced search version
        public (IList<Product> data, int total, int totalDisplay) GetPagedProducts(
            int pageIndex, int pageSize, string? order, DataTablesSearch search)
        {
            if (string.IsNullOrWhiteSpace(search.Value))
                return GetDynamic(null, order, null, pageIndex, pageSize, true);
            else
                return GetDynamic(
                    x => x.Name.Contains(search.Value) ||
                         x.Category.Contains(search.Value) ||
                         x.Details.Contains(search.Value) ||
                         x.Price.ToString().Contains(search.Value),
                    order, null, pageIndex, pageSize, true);
        }
    }
}
