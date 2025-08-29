using Autofac;
using CSE3200.Application.Features.Disasters.Commands;
using CSE3200.Application.Features.Disasters.Queries;
using CSE3200.Application.Features.Products.Commands;
using CSE3200.Application.Services;
using CSE3200.Domain;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Repositories;
using CSE3200.Domain.Services;
using CSE3200.Infrastructure;
using CSE3200.Infrastructure.Repositories;
using MediatR;


namespace CSE3200.Web
{
    public class WebModule : Module
    {
        private readonly string _connectionString;
        private readonly string _migrationAssembly;

        public WebModule(string connectionString, string migrationAssembly)
        {
            _connectionString = connectionString;
            _migrationAssembly = migrationAssembly;
        }

        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType<Infrastructure.ApplicationDbContext>().AsSelf()
                .WithParameter("connectionString", _connectionString)
                .WithParameter("migrationAssembly", _migrationAssembly)
                .InstancePerLifetimeScope();

            builder.RegisterType<ProductRepository>().As<IProductRepository>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ApplicationUnitOfWork>().As<IApplicationUnitOfWork>()
                .InstancePerLifetimeScope();


            builder.RegisterType<ProductService>().As<IProductService>()
         .InstancePerLifetimeScope();

            // builder.RegisterType<AddProductCommandHandler>().AsSelf();
            builder.RegisterType<AddProductCommand>().AsSelf();

            // Disaster-related registrations
            builder.RegisterType<DisasterRepository>().As<IDisasterRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DisasterService>().As<IDisasterService>()
                .InstancePerLifetimeScope();

            // Register command handlers with logger
            builder.RegisterType<AddDisasterCommandHandler>().As<IRequestHandler<AddDisasterCommand, Guid>>();
            builder.RegisterType<ApproveDisasterCommandHandler>().As<IRequestHandler<ApproveDisasterCommand>>();
            builder.RegisterType<RejectDisasterCommandHandler>().As<IRequestHandler<RejectDisasterCommand>>();
            builder.RegisterType<GetPendingApprovalsQueryHandler>().As<IRequestHandler<GetPendingApprovalsQuery, IList<Disaster>>>();

            // Add these registrations
            builder.RegisterType<DonationRepository>().As<IDonationRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DonationService>().As<IDonationService>()
                .InstancePerLifetimeScope();

            base.Load(builder);
        }

    }
}
