using Autofac;
using Autofac.Core;
using CSE3200.Application.Features.DisasterAlerts.Commands;
using CSE3200.Application.Features.DisasterAlerts.Queries;
using CSE3200.Application.Features.Disasters.Commands;
using CSE3200.Application.Features.Disasters.Queries;
using CSE3200.Application.Features.FAQs.Commands;
using CSE3200.Application.Features.FAQs.Queries;
using CSE3200.Application.Features.Products.Commands;
using CSE3200.Application.Features.Volunteers.Commands;
using CSE3200.Application.Features.Volunteers.Queries;
using CSE3200.Application.Services;
using CSE3200.Domain;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Repositories;
using CSE3200.Domain.Services;
using CSE3200.Infrastructure;
using CSE3200.Infrastructure.Repositories;
using CSE3200.Web.Services;
using MediatR;
using Microsoft.Extensions.Configuration;

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

            // Register ImageService
            builder.Register(ctx =>
            {
                var configuration = ctx.Resolve<IConfiguration>();
                return new ImageService(configuration);
            })
            .As<IImageService>()
            .InstancePerLifetimeScope();

            // Register other services
            builder.RegisterType<OtpService>().As<IOtpService>().InstancePerLifetimeScope();
            builder.RegisterType<EmailSender>().As<IEmailSender>().InstancePerLifetimeScope();

            // Register repositories
            builder.RegisterType<ProductRepository>().As<IProductRepository>()
                .InstancePerLifetimeScope();
            builder.RegisterType<DonationRepository>().As<IDonationRepository>()
                .InstancePerLifetimeScope();
            builder.RegisterType<VolunteerAssignmentRepository>().As<IVolunteerAssignmentRepository>()
                .InstancePerLifetimeScope();
            builder.RegisterType<FAQRepository>().As<IFAQRepository>()
                .InstancePerLifetimeScope();
            builder.RegisterType<DisasterAlertRepository>().As<IDisasterAlertRepository>()
                .InstancePerLifetimeScope();
            builder.RegisterType<DisasterRepository>().As<IDisasterRepository>()
                .InstancePerLifetimeScope();

            // Register Unit of Work
            builder.RegisterType<ApplicationUnitOfWork>().As<IApplicationUnitOfWork>()
                .InstancePerLifetimeScope();

            // Register services
            builder.RegisterType<ProductService>().As<IProductService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<DonationService>().As<IDonationService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<VolunteerAssignmentService>().As<IVolunteerAssignmentService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<FAQService>().As<IFAQService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<DisasterAlertService>().As<IDisasterAlertService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<DisasterService>().As<IDisasterService>()
                .InstancePerLifetimeScope();

            // Register command handlers
            builder.RegisterType<AddProductCommandHandler>().AsSelf();

            // FAQ command handlers
            builder.RegisterType<AddFAQCommandHandler>().As<IRequestHandler<AddFAQCommand, Guid>>();
            builder.RegisterType<UpdateFAQCommandHandler>().As<IRequestHandler<UpdateFAQCommand, bool>>();
            builder.RegisterType<ToggleFAQStatusCommandHandler>().As<IRequestHandler<ToggleFAQStatusCommand, bool>>();
            builder.RegisterType<GetFAQsQueryHandler>().As<IRequestHandler<GetFAQsQuery, IList<FAQ>>>();

            // Volunteer command handlers
            builder.RegisterType<AssignVolunteerCommandHandler>().As<IRequestHandler<AssignVolunteerCommand, Guid>>();
            builder.RegisterType<GetDisasterVolunteersQueryHandler>().As<IRequestHandler<GetDisasterVolunteersQuery, IList<VolunteerAssignment>>>();
            builder.RegisterType<RemoveVolunteerCommandHandler>().As<IRequestHandler<RemoveVolunteerCommand, bool>>();
            builder.RegisterType<UpdateVolunteerAssignmentCommandHandler>().As<IRequestHandler<UpdateVolunteerAssignmentCommand, bool>>();
            builder.RegisterType<GetAllVolunteerAssignmentsQueryHandler>().As<IRequestHandler<GetAllVolunteerAssignmentsQuery, IList<VolunteerAssignment>>>();

            // Disaster Alert command handlers
            builder.RegisterType<AddDisasterAlertCommandHandler>().As<IRequestHandler<AddDisasterAlertCommand, Guid>>();
            builder.RegisterType<UpdateDisasterAlertCommandHandler>().As<IRequestHandler<UpdateDisasterAlertCommand, bool>>();
            builder.RegisterType<ToggleDisasterAlertStatusCommandHandler>().As<IRequestHandler<ToggleDisasterAlertStatusCommand, bool>>();
            builder.RegisterType<GetDisasterAlertsQueryHandler>().As<IRequestHandler<GetDisasterAlertsQuery, IList<DisasterAlert>>>();

            // Disaster-related registrations
            builder.RegisterType<AddDisasterCommandHandler>().As<IRequestHandler<AddDisasterCommand, Guid>>();
            builder.RegisterType<ApproveDisasterCommandHandler>().As<IRequestHandler<ApproveDisasterCommand>>();
            builder.RegisterType<RejectDisasterCommandHandler>().As<IRequestHandler<RejectDisasterCommand>>();
            builder.RegisterType<GetPendingApprovalsQueryHandler>().As<IRequestHandler<GetPendingApprovalsQuery, IList<Disaster>>>();

            base.Load(builder);
        }
    }
}