using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Core.Commands
{
    class CancelRegistrationCommand : IRequest
    {
        public string RegistrationId { get; set; }
    }
}
