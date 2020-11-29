using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus
{
    public class IntegrationEvent
    {
        public virtual Guid Id { get; set; }

        public virtual DateTime? OccuredAt { get; set; }
    }
}
