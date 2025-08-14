using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using CSE3200.Domain;
using System;
using System.Collections.Generic;       
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public ProductService(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void AddProduct(Product product)
        {
            _unitOfWork.ProductRepository.Add(product); // Fixed access
            _unitOfWork.Save();
        }

        public void UpdateProduct(Product product)
        {
            var existingProduct = _unitOfWork.ProductRepository.GetById(product.Id); // Fixed access
            if (existingProduct == null)
            {
                throw new InvalidOperationException($"Product with ID {product.Id} not found");
            }

            // Update properties
            existingProduct.Name = product.Name;
            existingProduct.Category = product.Category;
            existingProduct.Price = product.Price;
            existingProduct.Rating = product.Rating;
            existingProduct.Details = product.Details;
            existingProduct.Quantity = product.Quantity;

            _unitOfWork.ProductRepository.Edit(existingProduct); // Fixed access
            _unitOfWork.Save();
        }

        public void DeleteProduct(Guid id)
        {
            var product = _unitOfWork.ProductRepository.GetById(id); // Fixed access
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {id} not found");
            }

            _unitOfWork.ProductRepository.Remove(product); // Fixed access
            _unitOfWork.Save();
        }

        public Product GetProduct(Guid id)
        {
            return _unitOfWork.ProductRepository.GetById(id); // Fixed access
        }

        public (IList<Product> data, int total, int totalDisplay) GetProducts(
            int pageIndex, int pageSize, string? order, DataTablesSearch search)
        {
            var result = _unitOfWork.ProductRepository.GetPagedProducts(
                pageIndex, pageSize, order, search);

            // Temporary logging
            Console.WriteLine($"Fetched {result.data.Count} products");
            if (result.data.Any())
            {
                Console.WriteLine($"First product: {result.data.First().Name}");
            }

            return result;
        }
    }
}


