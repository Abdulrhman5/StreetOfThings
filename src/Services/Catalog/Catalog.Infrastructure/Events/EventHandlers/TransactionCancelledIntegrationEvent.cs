using EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Infrastructure.Events.EventHandlers
{
    public class TransactionCancelledIntegrationEvent : IntegrationEvent
    {
        public Guid RegistrationId { get; set; }

        public DateTime CancelledAtUtc { get; set; }
    }
}
