using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Models
{
    public class ObjectReceiving : IEntity<ulong>
    { 
        public ulong ObjectReceivingId { get; set; }

        public ulong ObjectRegistrationId { get; set; }
        public ObjectRegistration ObjectRegistration { get; set; }

        public Guid RecipientLoginId { get; set; }
        public Login RecipientLogin { get; set; }

        public Guid GiverLoginId { get; set; }
        public Login GiverLogin { get; set; }

        public ulong? ObjectReturningId { get; set; }

        public ObjectReturning ObjectReturning { get; set; }

        public DateTime ReceivedAtUtc { get; set; }

        public float? HourlyCharge { get; set; }

        public ulong Id => ObjectReceivingId;
    }
}
