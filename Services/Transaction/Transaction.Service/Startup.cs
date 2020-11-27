using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizationService.Grpc;
using EventBus;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Transaction.Core;
using Transaction.Infrastructure;
using Unity;

namespace Transaction.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCore();
            services.AddInfrastructure(Configuration);
            services.AddControllersWithViews();
            services.AddControllers().AddNewtonsoftJson();

            var transactionConnection = Configuration.GetConnectionString("TransactionConnection");
            services.AddDbContext<Transaction.Infrastructure.Data.TransactionContext>(options => options.UseSqlServer(transactionConnection));
            services.AddHttpContextAccessor();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration["Services:Authorization"];
                    options.RequireHttpsMetadata = false;
                    options.Audience = "Transaction.Api";
                });


            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", pb =>
                {
                    pb.RequireRole(ClaimTypes.Role, "Admin");

                });
            });

            services.AddIntegrationEventService(new IntegrationEventOptions
            {
                HostName = "localhost",
                RetryCount = 5,
                SubscriptionClientName = "Transaction",
            });

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.ApplicationServices.AddIntegrationEvent<DummyEvent, DummyHandler>();

            ConfigDatabases.SeedUsersDatabase(app);
        }

        public void ConfigureContainer(IUnityContainer container)
        {

        }
    }

    public class DummyEvent : IntegrationEvent
    {
        public string X { get; set; }
    }

    public class DummyHandler : IIntegrationEventHandler<DummyEvent>
    {
        public async Task HandleEvent(DummyEvent integrationEvent)
        {
            Console.WriteLine(integrationEvent.X);
        }
    }
}
