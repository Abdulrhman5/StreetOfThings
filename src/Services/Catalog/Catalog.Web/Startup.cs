extern alias CatalogInfrastructure;
using CatalogInfrastructure::AuthorizationService.Grpc;
using CatalogService.Grpc;
using EventBus;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using Unity;
using CatalogInfrastructure::Catalog.Infrastructure.Events.EventHandlers;
using Catalog.ApplicationCore;
using CatalogInfrastructure::Catalog.Infrastructure;

namespace CatalogService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication();
            services.AddInfrastructure(Configuration);

            services.AddControllersWithViews();
            services.AddControllers().AddNewtonsoftJson();
            services.AddHttpContextAccessor();
            var catalogConnection = Configuration.GetConnectionString("CatalogConnection");
            Log.Information("Connection used for catalog " + catalogConnection);

            services.AddDbContext<CatalogInfrastructure.Catalog.Infrastructure.Data.CatalogContext>(opt =>
            {
                opt.UseSqlServer(catalogConnection, x => x.UseNetTopologySuite());
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog", Version = "v1" });
            });

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration["Services:Authorization"];
                    options.RequireHttpsMetadata = false;
                    options.Audience = "Catalog.Api";
                });

            services.AddIntegrationEventService(new IntegrationEventOptions
            {
                HostName = Configuration["Services:EventBus"],
                RetryCount = 5,
                SubscriptionClientName = "Catalog",
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", builder =>
                {
                    builder.RequireRole("Admin");
                });
            });
            services.AddGrpc(o =>
            {
                o.EnableDetailedErrors = true;

            });

            services.AddCors(options =>
            {
                var origins = Configuration.GetSection("AllowedCorsOrigins").Get<string[]>();
                options.AddPolicy(MyAllowSpecificOrigins, builder => builder.WithOrigins(origins).AllowAnyHeader()
                                                  .AllowAnyMethod());
            });

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            
            // Registration of the DI service
            services.AddGrpcClient<UsersGrpc.UsersGrpcClient>(options => {
                options.Address = new Uri("http://localhost:21000");
                options.ChannelOptionsActions.Add(channelOptions => channelOptions.Credentials = ChannelCredentials.Insecure);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
                endpoints.MapGrpcService<ObjectService>();
            });

            ConfigDatabases.SeedUsersDatabase(app);

            app.ApplicationServices.AddIntegrationEvent<NewRegistrationIntegrationEvent, NewRegistrationIntegrationEventHandler>();
            app.ApplicationServices.AddIntegrationEvent<TransactionCancelledIntegrationEvent,TransactionCancelledIntegrationEventHandler>();
            app.ApplicationServices.AddIntegrationEvent<TransactionReceivedIntegrationEvent,TransactionReceivedIntegrationEventHandler>();
            app.ApplicationServices.AddIntegrationEvent<TransactionReturnedIntegrationEvent,TransactionReturnedIntegrationEventHandler>();
        }

        public void ConfigureContainer(IUnityContainer container)
        {
            new ServiceUnityConfiguration().ConfigUnity(container);
        }
    }
}
