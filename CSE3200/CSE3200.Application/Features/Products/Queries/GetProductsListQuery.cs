using CSE3200.Domain.Entities;
using CSE3200.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Products.Queries
{
    public class GetProductsListQuery : IRequest<(IList<Product> data, int total, int totalDisplay)>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public DataTablesSearch Search { get; set; }
    }
}
