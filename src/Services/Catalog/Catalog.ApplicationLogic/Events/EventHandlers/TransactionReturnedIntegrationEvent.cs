using EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationLogic.Events.EventHandlers
{
    public class TransactionReturnedIntegrationEvent : IntegrationEvent
    {
        public Guid ReturnIdId { get; set; }

        public Guid RegistrationId { get; set; }

        public DateTime ReturnedAtUtc { get; set; }
    }
}
