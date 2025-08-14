using CSE3200.Application.Features.Products.Commands;
using CSE3200.Domain.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Products.Commands
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IProductService _productService;

        public UpdateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = _productService.GetProduct(request.Id);
            if (product == null)
            {
                throw new Exception($"Product with ID {request.Id} not found.");
            }

            // Update all properties
            product.Name = request.Name;
            product.Category = request.Category;
            product.Price = request.Price;
            product.Rating = request.Rating;
            product.Details = request.Details;
            product.Quantity = request.Quantity;

            _productService.UpdateProduct(product);
        }
    }
}
