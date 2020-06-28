using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace EventBus
{
    public static class EventBusServiceCollectionExtensions
    {

        public static IServiceCollection AddIntegrationEventService(this IServiceCollection services, IntegrationEventOptions options)
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = options.HostName,
                    DispatchConsumersAsync = true,
                };

                var logger = sp.GetRequiredService<ILogger<RabbitMqPresistentConnection>>();
                return new RabbitMqPresistentConnection(factory, logger, options.RetryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>(sp =>
             {
                 return new InMemoryEventBusSubscriptionsManager();
             });

            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var rabbitConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var subscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                var container = sp.GetRequiredService<IUnityContainer>();
                var bus =  new EventBusRabbitMQ(rabbitConnection, logger, subscriptionsManager,container, options.SubscriptionClientName, options.SendingRetryCount);
                return bus;
            });

            return services;
        } 
        
        public static IServiceProvider AddIntegrationEvent<TEvent, THandler> (this IServiceProvider serviceProvider)
             where TEvent : IntegrationEvent
             where THandler : IIntegrationEventHandler<TEvent>

        {
            var bus = serviceProvider.GetRequiredService<IEventBus>();
            bus.Subscribe<TEvent, THandler>();
            return serviceProvider;
        }
    }

    public class IntegrationEventOptions
    {
        public string HostName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int RetryCount { get; set; }

        public int SendingRetryCount { get; set; }

        public string SubscriptionClientName { get; set; }

    }
}
