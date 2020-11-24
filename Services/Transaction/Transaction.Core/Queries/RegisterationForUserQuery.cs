using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Transaction.Core.Dtos;

namespace Transaction.Core.Queries
{
    class RegisterationForUserQuery : PagingArguments, IRequest<List<RegistrationDto>>
    {
        public RegistrationForUserType RegistrationType { get; set; }
    }

    public enum RegistrationForUserType
    {
        UserAsRecipient,
        UserAsOwner,
        All
    }
}
