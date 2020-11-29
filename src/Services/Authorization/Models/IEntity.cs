using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public interface IEntity<T>
    {
        public T Id { get; }
    }
}
