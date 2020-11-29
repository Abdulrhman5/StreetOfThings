using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Domain
{
    public interface IEntity<T>
    {
        public T Id { get; }
    }
}
