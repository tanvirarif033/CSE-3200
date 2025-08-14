using CSE3200.Application.Features.Products.Commands;
using  CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Products.Commands
{
    public class AddProductCommandHandler : IRequestHandler<AddProductCommand>
    {
        private readonly IProductService _productService;

        public AddProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Category = request.Category,
                Price = request.Price,
                Rating = request.Rating,
                Details = request.Details,
                Quantity = request.Quantity
            };
            _productService.AddProduct(product);
        }
    }
}
