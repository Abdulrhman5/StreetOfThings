using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Domain
{
    public abstract class BaseEntity<T> : IEntity<T>
    {
        public List<BaseDomainEvent> Events = new List<BaseDomainEvent>();

        public abstract T Id { get; }
    }
}
