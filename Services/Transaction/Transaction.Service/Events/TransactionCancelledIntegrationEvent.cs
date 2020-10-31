using EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.BusinessLogic.Events
{
    class TransactionCancelledIntegrationEvent : IntegrationEvent
    {
        public Guid RegistrationId { get; set; }

        public DateTime CancelledAtUtc { get; set; }
    }
}
