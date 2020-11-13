﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Transaction.Domain.Entities
{
    public class ObjectRegistration : BaseEntity<Guid>
    {
        public Guid ObjectRegistrationId { get; set; }

        public DateTime RegisteredAtUtc { get; set; }

        public DateTime ExpiresAtUtc { get; set; }

        public TimeSpan? ShouldReturnItAfter { get; set; }

        public ObjectRegistrationStatus Status { get; set; }

        public Guid RecipientLoginId { get; set; }
        public Login RecipientLogin { get; set; }

        public ObjectReceiving ObjectReceiving { get; set; }

        public int ObjectId { get; set; }
        public OfferedObject Object { get; set; }

        public List<TransactionToken> Tokens { get; set; }

        public override Guid Id => ObjectRegistrationId;
    }

    public enum ObjectRegistrationStatus
    {
        OK,
        Canceled,
    }
}
