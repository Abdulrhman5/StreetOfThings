using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Models
{
    public class ObjectReceiving : IEntity<Guid>
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

        public Guid Id => ObjectReceivingId;
    }
}
