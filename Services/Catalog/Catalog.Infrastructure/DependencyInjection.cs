using Catalog.ApplicationCore.Interfaces;
using Catalog.ApplicationLogic.Infrastructure;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
namespace Catalog.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IObjectServiceResolver, ObjectServiceResolver>();
            services.AddTransient<IUserDataManager, UserDataManager>();
            services.AddTransient<IImageSaver, ImageSaverAdapter>();
            services.AddTransient<HostingHelpers.IImageSaver, HostingHelpers.ImageSaver>();
            services.AddTransient(typeof(IOwnershipAuthorization<,>), typeof(OwnershipAuthorization<,>));
            services.AddTransient<IPhotoUrlConstructor, PhotoUrlConstructor>();
            services.AddTransient(typeof(IRepository<,>), typeof(GenericRepository<,>));
            return services;
        }
    }
}
