using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Core.Dtos
{
    public class CreateReturningResultDto
    {
        public Guid RegistrationId { get; set; }

        public Guid ReceivingId { get; set; }

        public Guid ReturningId { get; set; }

        public DateTime RegisteredAtUtc { get; set; }

        public DateTime ReceivedAtUtc { get; set; }

        public DateTime? ShouldBeReturnedAtUtc { get; set; }

        public DateTime ReturnedAtUtc { get; set; }

        public TimeSpan ReturnedAfter { get; set; }

        public TimeSpan Late { get; set; }

        public float? ShouldPay { get; set; }

    }
}
