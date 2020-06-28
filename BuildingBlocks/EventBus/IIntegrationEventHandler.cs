using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBus
{
    public interface IIntegrationEventHandler<TEvent>
    {
        public Task HandleEvent(TEvent integrationEvent);
    }
}
