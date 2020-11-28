using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Transaction.Service.Dtos;

namespace Transaction.Core.Commands
{
    public class CreateRegistrationCommand : IRequest<CommandResult<CreateRegistrationResultDto>>
    {
        public int ObjectId { get; set; }

        public int? ShouldReturnAfter { get; set; }
    }
}
