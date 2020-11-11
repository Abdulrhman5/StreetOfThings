using Catalog.ApplicationCore.Interfaces;
using Catalog.ApplicationLogic.Infrastructure;
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
            services.AddTransient(typeof(IOwnershipAuthorization<,>), typeof(IOwnershipAuthorization<,>));
            services.AddTransient<IPhotoUrlConstructor, PhotoUrlConstructor>();
            return services;
        }
    }
}
