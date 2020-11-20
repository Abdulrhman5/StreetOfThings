using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Transaction.Core.Dtos;

namespace Transaction.Core.Commands
{
    public class CreateReceivingCommand : IRequest<CreateReceivingResultDto>
    {
        public string RegistrationToken { get; set; }
    }
}
