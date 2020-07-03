using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Models
{
    public class ObjectReturning
    {
        public ulong ObjectReturningId { get; set; }

        public DateTime ReturnedAtUtc { get; set; }
            
        public ulong ObjectReceivingId { get; set; }

        public ObjectReceiving ObjectReceiving { get; set; }
    }
}
