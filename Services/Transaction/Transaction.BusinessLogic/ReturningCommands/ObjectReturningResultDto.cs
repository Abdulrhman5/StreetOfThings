using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.BusinessLogic.ReturningCommands
{
    public class ObjectReturningResultDto
    {
        public Guid RegistrationId { get; set; }

        public Guid ReceivingId { get; set; }

        public Guid ReturningId { get; set; }

        public TimeSpan ReturnedAfter { get; set; }

        public TimeSpan Late { get; set; }

        public float? ShouldPay { get; set; }
    }
}
