using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Transaction.Core.Exceptions;

namespace Transaction.Core.Commands
{
    class CancelRegistrationCommand : IRequest<CommandResult>
    {
        public string RegistrationId { get; set; }
    }
}
