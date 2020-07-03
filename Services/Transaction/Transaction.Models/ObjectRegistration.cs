using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Models
{
    public class ObjectRegistration : IEntity<ulong>
    {
        public ulong ObjectRegistrationId { get; set; }

        public DateTime RegisteredAtUtc { get; set; }

        public DateTime ExpiresAtUtc { get; set; }

        public TimeSpan ShouldReturnItAfter { get; set; }

        public ObjectRegistrationStatus Status { get; set; }

        public Guid RecipientLoginId { get; set; }
        public Login RecipientLogin { get; set; }

        public ObjectReceiving ObjectReceiving { get; set; }

        public ulong Id => ObjectRegistrationId;
    }

    public enum ObjectRegistrationStatus
    {
        OK,
        Canceled,
    }
}
