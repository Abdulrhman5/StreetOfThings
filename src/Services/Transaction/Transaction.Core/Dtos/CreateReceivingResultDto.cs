using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Core.Dtos
{
    public class CreateReceivingResultDto
    {
        public Guid RegistrationId { get; set; }

        public DateTime ReceivedAtUtc { get; set; }

        public int ObjectId { get; set; }

        public TimeSpan? ShouldBeReturnedAfterReceving { get; set; }
    }
}
