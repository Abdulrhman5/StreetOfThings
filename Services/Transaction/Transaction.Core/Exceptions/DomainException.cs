using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Core.Exceptions
{
    public class DomainException : Exception
    {
        private ErrorMessage _message;

        public ErrorMessage ReadyToRenderMessage => _message;

        public DomainException (ErrorMessage readyToRenderMessage)
        {
            _message = readyToRenderMessage;
        }
    }
}
