using CSE3200.Domain;
using CSE3200.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Infrastructure
{
    public class ApplicationUnitOfWork : UnitOfWork, IApplicationUnitOfWork
    {
        public ApplicationUnitOfWork(ApplicationDbContext context,
           IProductRepository productRepository) : base(context)
        {

            ProductRepository = productRepository;
        }


        public IProductRepository ProductRepository { get; private set; }

    }
}
