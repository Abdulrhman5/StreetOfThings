using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;
using Transaction.Service.Dtos;

namespace Transaction.Core.Commands
{
    public class CreateRegistrationCommandHandler : IRequestHandler<CreateRegistrationCommand, CreateRegistrationResultDto>
    {
        public async Task<CreateRegistrationResultDto> Handle(CreateRegistrationCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();    
        }
    }
}
