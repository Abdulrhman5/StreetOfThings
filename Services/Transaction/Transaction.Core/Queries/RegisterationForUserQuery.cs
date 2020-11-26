using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Transaction.Core.Dtos;

namespace Transaction.Core.Queries
{
    public class RegisterationForUserQuery : PagingArguments, IRequest<List<RegistrationDto>>
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
