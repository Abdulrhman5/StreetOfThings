using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Core.Interfaces
{
    public interface IRandomStringGenerator
    {
        string GenerateIdentifier(int length, CharsInToken charsInToken);
    }

    public enum CharsInToken
    {
        Numeric,
        CapitalSmallNumeric_,
        CapitalNumeric
    }
}
