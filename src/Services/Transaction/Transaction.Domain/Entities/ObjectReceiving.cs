using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Domain.Entities
{
    public class ObjectReceiving : BaseEntity<Guid>
    { 
        public Guid ObjectReceivingId { get; set; }

        public Guid ObjectRegistrationId { get; set; }
        public ObjectRegistration ObjectRegistration { get; set; }

        public Guid RecipientLoginId { get; set; }
        public Login RecipientLogin { get; set; }

        public Guid GiverLoginId { get; set; }
        public Login GiverLogin { get; set; }

        public ObjectReturning ObjectReturning { get; set; }

        public DateTime ReceivedAtUtc { get; set; }

        public float? HourlyCharge { get; set; }

        public List<TransactionToken> Tokens { get; set; }

        public override Guid Id => ObjectReceivingId;
    }
}
