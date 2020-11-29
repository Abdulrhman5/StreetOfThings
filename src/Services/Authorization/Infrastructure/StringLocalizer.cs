using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public interface IStringLocalizer
    {
        string this[string index] { get; }
    }
}
