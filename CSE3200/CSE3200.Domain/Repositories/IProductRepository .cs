using CSE3200.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Domain.Repositories
{
    public interface IProductRepository : IRepository<Product, Guid>
    {
        (IList<Product> data, int total, int totalDisplay) GetPagedProducts(
            int pageIndex, int pageSize, string? order, DataTablesSearch search);
    }
}
