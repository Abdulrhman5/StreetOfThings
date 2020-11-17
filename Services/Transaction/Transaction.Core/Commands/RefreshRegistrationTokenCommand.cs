using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Transaction.Service.Dtos;

namespace Transaction.Core.Commands
{
    class RefreshRegistrationTokenCommand : IRequest<RegistrationTokenResultDto>
    {
        public int ObjectId { get; set; }
    }
}
