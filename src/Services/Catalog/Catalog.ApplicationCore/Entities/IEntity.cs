using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationCore.Entities
{
    public interface IEntity<T>
    {
        public T Id { get; }
    }
}
