using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Models
{
    public class TransactionToken : IEntity<Guid>
    {
        public Guid TransactionTokenId { get; set; }

        public TokenType Type { get; set; }

        public DateTime IssuedAtUtc { get; set; }

        public DateTime UseAfterUtc { get; set; }

        public DateTime UseBeforeUtc { get; set; }

        public TokenStatus Status { get; set; }

        public string Token { get; set; }


        public Guid? ReceivingId { get; set; }
        public ObjectReceiving Receiving { get; set; }


        public Guid? RegistrationId { get; set; }
        public ObjectRegistration Registration { get; set; }

        public Guid Id => TransactionTokenId;
    }

    public enum TokenType
    {
        Receiving,
        Returning,
    }

    public enum TokenStatus
    {
        Ok, 
        Resolved
    }
}
