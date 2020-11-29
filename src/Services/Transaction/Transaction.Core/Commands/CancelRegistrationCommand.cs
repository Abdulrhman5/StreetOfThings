using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Core.Commands
{
    public class CancelRegistrationCommand : IRequest<CommandResult>
    {
        public string RegistrationId { get; set; }
    }
}
