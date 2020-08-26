using Microsoft.Extensions.DependencyInjection;
using MobileApiGateway.Services;
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
            servicesCollection.AddTransient<CatalogService>();

        }
    }
}
