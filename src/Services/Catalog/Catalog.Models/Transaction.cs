using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Models
{
    public class Transaction : IEntity<Guid>
    {
        public Guid TransactionId { get; set; }

        public DateTime RegisteredAtUtc { get; set; }

        public Guid? ReceivingId { get; set; }

        public DateTime? ReceivedAtUtc { get; set; }

        public Guid? ReturnId { get; set; }
        public DateTime? ReturnedAtUtc { get; set; }

        public float? Rating { get; set; }

        public TransactionStatus Status { get; set; }

        public Guid ReceipientId { get; set; }
        public User Receipient { get; set; }

        public int ObjectId { get; set; }
        public OfferedObject Object { get; set; }
              

        public Guid Id => TransactionId;
    }

    public enum TransactionStatus
    {
        Ok, Cancelled
    }
}
