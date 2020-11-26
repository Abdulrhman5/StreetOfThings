using FluentValidation;
using System;
using Transaction.Core.Commands;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;
using System.Linq;

namespace Transaction.Core.Validations
{
    public class CreateRegistrationValidator : AbstractValidator<CreateRegistrationCommand>
    {
        public CreateRegistrationValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(reg => reg)
                .NotNull()
                .WithErrorMessage(new ErrorMessage()
                {
                    ErrorCode = "TRANSACTION.OBJECT.RESERVE.NULL",
                    Message = "Please send a valid information",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
        }
    }
}
