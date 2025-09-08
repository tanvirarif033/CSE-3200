using CSE3200.Domain;
using CSE3200.Domain.Repositories;
using CSE3200.Domain.Services;
using CSE3200.Infrastructure.Repositories;
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
           IProductRepository productRepository,
           IDisasterRepository disasterRepository,
           IDonationRepository donationRepository,
           IVolunteerAssignmentRepository volunteerAssignmentRepository,
           IFAQRepository faqRepository,
           IDisasterAlertRepository disasterAlertRepository) : base(context)
        {
            ProductRepository = productRepository;
            DisasterRepository = disasterRepository;
            DonationRepository = donationRepository;
            VolunteerAssignmentRepository = volunteerAssignmentRepository;
            FAQRepository = faqRepository;
            DisasterAlertRepository = disasterAlertRepository;
        }

        public IProductRepository ProductRepository { get; private set; }
        public IDisasterRepository DisasterRepository { get; private set; }
        public IDonationRepository DonationRepository { get; private set; }
        public IVolunteerAssignmentRepository VolunteerAssignmentRepository { get; private set; }
        public IFAQRepository FAQRepository { get; private set; }
        public IDisasterAlertRepository DisasterAlertRepository { get; private set; }

    }
}
