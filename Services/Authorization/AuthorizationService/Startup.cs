using DataAccessLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using Unity;
using AuthorizationService.AppStart;
using Microsoft.OpenApi.Models;
using AuthorizationService.Identity;
using System;
using Serilog;
using System.Text;
using AuthorizationService.Grpc;

namespace AuthorizationService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();


            // Get Connection strings 
            var usersCString = Configuration.GetConnectionString("UsersConnection");
            Log.Information("Connection Used for Users" + usersCString);
            var configurationsCString = Configuration.GetConnectionString("ConfigurationsConnection");
            var persistedCString = Configuration.GetConnectionString("PersistedConnection");

            services.AddDbContext<AuthorizationContext>(options => options.UseSqlServer(usersCString, x => x.UseNetTopologySuite()));

            services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AuthorizationContext>();

            services.AddIdentityServer()
                .AddInMemoryClients(ClientsConfig.Clients)
                .AddInMemoryApiResources(ClientsConfig.Apis)
                .AddInMemoryIdentityResources(ClientsConfig.IdentityResources)
                .AddDeveloperSigningCredential()
                .AddAspNetIdentity<AppUser>()
                .AddResourceOwnerValidator<ResourceOwnerValidator>()
                .AddProfileService<CustomProfileService>();

            services.AddRazorPages();
            services.AddAuthorization();

            services.AddCustomAuthentication();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity", Version = "v1" });
            });

            services.AddGrpc(o =>
            {
                o.EnableDetailedErrors = true;
                
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseIdentityServer();

            app.UseStaticFiles();


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity API");
            });


            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();

                endpoints.MapGrpcService<UserService>();
            });

            ConfigDatabases.SeedUsersDatabase(app);
        }

        public void ConfigureContainer(IUnityContainer container)
        {
            new UnityConfiguration().ConfigUnity(container);
        }
    }
}
