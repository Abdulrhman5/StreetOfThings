using Catalog.ApplicationCore.Interfaces;
using EventBus;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Infrastructure.Services
{
    public interface IObjectServiceResolver
    {
        IObjectService ResolveObjectService();
    }

    class ObjectServiceResolver : IObjectServiceResolver
    {
        private IServiceProvider _serviceProvider;
        public ObjectServiceResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IObjectService ResolveObjectService()
        {
            return new EventedObjectService(_serviceProvider.GetRequiredService<IObjectService>(),
                _serviceProvider.GetRequiredService<IEventBus>());
        }
    }
}
