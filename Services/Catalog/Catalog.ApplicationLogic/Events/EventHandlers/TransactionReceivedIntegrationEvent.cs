using EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationLogic.Events
{
    public class TransactionReceivedIntegrationEvent : IntegrationEvent
    {
        public Guid RegistrationId { get; set; }

        public Guid ReceivingId { get; set; }
        public DateTime ReceivedAtUtc { get; set; }

    }
}
