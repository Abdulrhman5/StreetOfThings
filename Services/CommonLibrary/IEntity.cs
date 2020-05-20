using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary
{
    public interface IEntity<T>
    {
        public T Id { get; }
    }
}
