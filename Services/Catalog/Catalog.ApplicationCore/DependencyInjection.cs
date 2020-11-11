using Catalog.ApplicationCore.Interfaces;
using Catalog.ApplicationCore.Services;
using Catalog.ApplicationCore.Services.ObjectServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationCore
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<ObjectAdderDeleter>();
            services.AddTransient<ObjectGetter>();
            services.AddTransient<IObjectQueryHelper,ObjectQueryHelper>();
            services.AddTransient<IObjectService, ObjectServiceFacade>();

            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<IObjectImpressionsService, ObjectImpressionsService>();
            services.AddTransient<IObjectViewsService, ObjectViewsService>();
            services.AddTransient<IObjectLikeService, ObjectLikeService>();
            services.AddTransient<ITagService, TagService>();
            services.AddTransient<IObjectPhotoService, ObjectPhotoService>();
            return services;
        }
    }
}
