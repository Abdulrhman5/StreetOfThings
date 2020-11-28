using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Transaction.Core.Commands;
using Transaction.Core.Interfaces;
using System.Linq;
using Transaction.Domain.Entities;

namespace Transaction.Core.Validations
{
    class RefreshRegistrationTokenValidator : AbstractValidator<RefreshRegistrationTokenCommand>
    {
        public RefreshRegistrationTokenValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(refreshToken => refreshToken)
                .NotNull()
                .WithErrorMessage(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.REFRESH.NULL",
                    Message = "Please send valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });

        }
    }
}
