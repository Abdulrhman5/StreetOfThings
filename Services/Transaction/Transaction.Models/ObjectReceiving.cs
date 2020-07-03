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

        public DateTime ReceivedAtUtc { get; set; }

        public ulong Id => ObjectReceivingId;
    }
}
