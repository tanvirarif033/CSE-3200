using Autofac;
using CSE3200.Application.Features.Products.Commands;
using CSE3200.Application.Services;
using CSE3200.Domain;
using CSE3200.Domain.Repositories;
using CSE3200.Domain.Services;
using CSE3200.Infrastructure;
using CSE3200.Infrastructure.Repositories;


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



            base.Load(builder);
        }

    }
}
