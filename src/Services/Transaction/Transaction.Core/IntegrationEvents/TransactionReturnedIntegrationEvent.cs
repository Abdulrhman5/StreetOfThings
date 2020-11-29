using EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Core.IntegrationEvents
{
    class TransactionReturnedIntegrationEvent : IntegrationEvent
    {
        public Guid ReturnIdId { get; set; }

        public Guid RegistrationId { get; set; }

        public DateTime ReturnedAtUtc { get; set; }
    }
}
