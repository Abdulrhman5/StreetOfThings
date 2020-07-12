using EventBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.Events.EventHandlers
{
    public class NewRegistrationIntegrationEventHandler : IIntegrationEventHandler<NewRegistrationIntegrationEvent>
    {   
        public async Task HandleEvent(NewRegistrationIntegrationEvent integrationEvent)
        {
           
        }
    }
}
