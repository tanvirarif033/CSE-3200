using Autofac;
using Autofac.Extensions.DependencyInjection;
using CSE3200.Application.Features.Products.Commands;
using CSE3200.Infrastructure;
using CSE3200.Infrastructure.Extensions;
using CSE3200.Web;
using CSE3200.Web.Data;
using CSE3200.Web.Hubs;
using CSE3200.Web.Services;

using Microsoft.AspNetCore.Authentication.Google; // for clarity
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System.Reflection;

//  Bootstrap Logger
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateBootstrapLogger();

try
{
    Log.Information("Application is starting.");

    var builder = WebApplication.CreateBuilder(args);

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                           ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    var migrationAssembly = Assembly.GetExecutingAssembly();

    //  Autofac
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        containerBuilder.RegisterModule(new WebModule(connectionString, migrationAssembly?.FullName));
    });

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
         options.UseSqlServer(connectionString, (x) => x.MigrationsAssembly(migrationAssembly)));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    // Razor Pages + MVC
    builder.Services.AddRazorPages();
    builder.Services.AddControllersWithViews();


    // Add SignalR
    builder.Services.AddSignalR();

    // Serilog
    builder.Host.UseSerilog((context, lc) => lc
       .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(builder.Configuration)
    );

    // MediatR
    builder.Services.AddMediatR(cfg => {
        cfg.RegisterServicesFromAssembly(migrationAssembly);
        cfg.RegisterServicesFromAssembly(typeof(AddProductCommand).Assembly);
    });

    // Identity (??????? ?????????)
    builder.Services.AddIdentity();
    builder.Services.AddPolicy();

    // ? Google Authentication — ?? ?????? ?????? Build() ?? ???
    builder.Services
        .AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
            options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
            // ?????: options.AuthorizationEndpoint += "?prompt=select_account";
        });

    // Add distributed cache (memory cache for development)
    builder.Services.AddDistributedMemoryCache();

    // Add email configuration
    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

    // Register services
    builder.Services.AddScoped<IOtpService, OtpService>();
    builder.Services.AddTransient<IEmailSender, EmailSender>();
    // Add HttpClient factory (you already have this, keep it)
    builder.Services.AddHttpClient();

    // Register Maps Service as a typed client (FIX)
    builder.Services.AddHttpClient<IMapsService, GoogleMapsService>();


    var app = builder.Build();

    // Pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();

    // Order matters
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapStaticAssets();

    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();
    // Add public FAQ route
    app.MapControllerRoute(
        name: "faq",
        pattern: "faq",
        defaults: new { controller = "PublicFAQ", action = "Index" });

    app.MapRazorPages().WithStaticAssets();


    // Add SignalR hub mapping
    app.MapHub<ChatHub>("/chatHub");

    app.Run();

    Log.Information("Application started successfully.");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start.");
}
finally
{
    Log.CloseAndFlush();
}
