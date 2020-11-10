using EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Events.EventHandlers
{
    public class NewRegistrationIntegrationEvent : IntegrationEvent
    {
        public Guid RegistrationId { get; set; }

        public int ObjectId { get; set; }

        public string RecipiantId { get; set; }

        public DateTime RegisteredAt { get; set; }

        public bool ShouldReturn { get; set; }
    }
}
