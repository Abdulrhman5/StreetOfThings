using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizationService.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Grpc.Net.Client;
using Grpc.Core;
using Ocelot.Values;
using Unity;
using AutoMapper;

namespace AdministrationGateway
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddOcelot();
            services.AddHttpContextAccessor();
            services.AddAutoMapper(typeof(Startup));
            
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration["Services:Authorization"];
                    options.RequireHttpsMetadata = false;
                    options.Audience = "AdminGateway";
                });

            services.AddAuthorization(options=>
            { 
                options.AddPolicy("Admin",pb =>
                {
                    pb.RequireRole(ClaimTypes.Role, "Admin");

                });
            }); 

            services.AddCors(options =>
            {
                var origins = Configuration.GetSection("AllowedCorsOrigins").Get<string[]>();
                options.AddPolicy(MyAllowSpecificOrigins, builder => builder.WithOrigins(origins).AllowAnyHeader()
                                                  .AllowAnyMethod());
            });
            // Enable support for unencrypted HTTP2  
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            // Registration of the DI service
            services.AddGrpcClient<UsersGrpc.UsersGrpcClient>(options => {
                options.Address = new Uri("http://localhost:21000");
                options.ChannelOptionsActions.Add(channelOptions => channelOptions.Credentials = ChannelCredentials.Insecure);
            }); 
            services.AddGrpcClient<CatalogService.Grpc.ObjectsGrpc.ObjectsGrpcClient>(options => {
                options.Address = new Uri("http://localhost:21001");
                options.ChannelOptionsActions.Add(channelOptions => channelOptions.Credentials = ChannelCredentials.Insecure);
            });
            services.ConfigureProjectServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });


            await app.UseOcelot();
        }

    }
}
