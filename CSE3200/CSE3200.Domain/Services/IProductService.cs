using CSE3200.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Domain.Services
{
    public interface IProductService
    {
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(Guid id);
        Product GetProduct(Guid id);
        (IList<Product> data, int total, int totalDisplay) GetProducts(
            int pageIndex, int pageSize, string? order, DataTablesSearch search);
    }
}
