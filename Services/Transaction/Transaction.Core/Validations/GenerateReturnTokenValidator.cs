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
                .WithErrorMessage(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.NULL",
                    Message = "Please provide valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });

            RuleFor(tokenRequest => tokenRequest.RegistrationId)
                .NotNull()
                .NotEmpty()
                .WithErrorMessage(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.NULL",
                    Message = "Please provide valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });

            RuleFor(tokenRequest => tokenRequest.RegistrationId)
                .Must(registrationId => Guid.TryParse(registrationId, out var result))
                .WithErrorMessage(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.NULL",
                    Message = "Please provide valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
        }
    }
}
