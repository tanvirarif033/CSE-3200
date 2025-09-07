using CSE3200.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Domain
{
    public interface IApplicationUnitOfWork : IUnitOfWork
    {

        public IProductRepository ProductRepository { get; }
        public  IDisasterRepository DisasterRepository { get; }
        public IDonationRepository DonationRepository { get; }
        public IVolunteerAssignmentRepository VolunteerAssignmentRepository { get; }
        public IFAQRepository FAQRepository { get; }

    }
}
