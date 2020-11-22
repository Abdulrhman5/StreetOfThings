using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Transaction.Core.Commands;

namespace Transaction.Core.Validations
{
    class GenerateReturnTokenValidator : AbstractValidator<GenerateReturnTokenCommand>
    {

        public GenerateReturnTokenValidator()
        {
            RuleFor(tokenRequest => tokenRequest)
                .NotNull()
                .WithMessage("TRANSACTION.TOKEN.GENERATE.RETURN.NULL");

            RuleFor(tokenRequest => tokenRequest.RegistrationId)
                .NotNull()
                .NotEmpty()
                .WithMessage("TRANSACTION.TOKEN.GENERATE.RETURN.NULL");

            RuleFor(tokenRequest => tokenRequest.RegistrationId)
                .Must(registrationId => Guid.TryParse(registrationId, out var result))
                .WithMessage("TRANSACTION.TOKEN.GENERATE.INVALID.ID");
        }
    }
}
