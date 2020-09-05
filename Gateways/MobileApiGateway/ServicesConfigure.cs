using Microsoft.Extensions.DependencyInjection;
using MobileApiGateway.Infrastructure;
using MobileApiGateway.Services;
using MobileApiGateway.Services.ObjectCommentServices;
using MobileApiGateway.Services.TransactionServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace MobileApiGateway
{
    public static class ServicesConfigure
    {
        public static void ConfigureIoc(this IServiceCollection servicesCollection)
        {
            servicesCollection.AddTransient<HttpClientHelpers>();
            servicesCollection.AddTransient<Services.CatalogService>();
            servicesCollection.AddHttpClient<CommentAggregator>();
            servicesCollection.AddTransient<UserService>();
            servicesCollection.AddHttpClient<TransactionService>();
            servicesCollection.AddTransient<ObjectService>();
            servicesCollection.AddTransient<CurrentUserCredentialsGetter>();
            servicesCollection.AddHttpClient<CatalogAggregator>();
        }
    }
}
