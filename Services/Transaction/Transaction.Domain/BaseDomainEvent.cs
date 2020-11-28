using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Domain
{
    public class BaseDomainEvent : INotification
    {
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;

    }
}
