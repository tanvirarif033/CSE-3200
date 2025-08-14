using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Products.Queries
{
    public class GetProductsListQueryHandler : IRequestHandler<GetProductsListQuery, (IList<Product> data, int total, int totalDisplay)>
    {
        private readonly IProductService _productService;

        public GetProductsListQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<(IList<Product> data, int total, int totalDisplay)> Handle(
            GetProductsListQuery request, CancellationToken cancellationToken)
        {
            return _productService.GetProducts(
                request.PageIndex,
                request.PageSize,
                request.Order,
                request.Search);
        }
    }
}
