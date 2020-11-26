using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Transaction.Service.Dtos;

namespace Transaction.Core.Commands
{
    public class RefreshRegistrationTokenCommand : IRequest<CommandResult<RegistrationTokenResultDto>>
    {
        public int ObjectId { get; set; }
    }
}
