using EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Infrastructure.Events
{
    public class ObjectDeletedIntegrationEvent : IntegrationEvent
    {
        public int ObjectId { get; set; }

        public DateTime DeletedAtUtc { get; set; }
    }
}
