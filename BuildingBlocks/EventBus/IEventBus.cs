using System;

namespace EventBus
{
    public interface IEventBus
    {
        void Subscribe<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>;

        void Unsubscribe<TEvent, THandler>()
            where TEvent: IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>;

        void Publish(IntegrationEvent evnt);
    }
}
