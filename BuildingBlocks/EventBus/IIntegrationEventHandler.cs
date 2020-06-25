using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus
{
    public interface IIntegrationEventHandler<TEvent>
    {
        public void HandleEvent(TEvent integrationEvent);
    }
}
