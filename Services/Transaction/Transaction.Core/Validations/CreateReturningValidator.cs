using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Transaction.Core.Commands;

namespace Transaction.Core.Validations
{
    public class CreateReturningValidator : AbstractValidator<CreateReturningCommand>
    {
        public CreateReturningValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(returning => returning)
                .NotNull()
                .WithErrorMessage(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.NULL",
                    Message = "Please send valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });
            RuleFor(returning => returning.ReturningToken)
                .NotNull()
                .NotEmpty()
                .WithErrorMessage(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.NULL",
                    Message = "Please send valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });
        }
    }
}
