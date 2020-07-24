using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.BusinessLogic.ReceivingCommands
{
    public class ObjectReceivingResultDto
    {
        public Guid RegistrationId { get; set; }

        public DateTime ReceivedAtUtc { get; set; }

        public int ObjectId { get; set; }

        public TimeSpan? ShouldBeReturnedAfterReceving { get; set; }
    }
}
