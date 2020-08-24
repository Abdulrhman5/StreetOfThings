using AdministrationGateway.Services.TransactionServices;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Values;
using System;
using Unity;

namespace AdministrationGateway
{
    internal static class ProjectServices
    {
        internal static void ConfigureProjectServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpClient<TransactionService>();
            serviceCollection.AddTransient<ObjectService>();
            serviceCollection.AddTransient<UserService>();
            serviceCollection.AddTransient<HttpClientHelpers>();
            serviceCollection.AddTransient<HttpResponseMessageConverter>();

        }
    }
}