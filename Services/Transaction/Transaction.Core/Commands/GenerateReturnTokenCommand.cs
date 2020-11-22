using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Transaction.Core.Dtos;

namespace Transaction.Core.Commands
{
    public class GenerateReturnTokenCommand : IRequest<GenerateReturnTokenResultDto>
    {
        public string RegistrationId { get; set; }
    }
}
