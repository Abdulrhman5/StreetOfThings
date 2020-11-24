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
            RuleFor(returning => returning).NotNull().WithMessage("TRANSACTION.RETURNING.ADD.NULL");
            RuleFor(returning => returning).NotNull().NotEmpty().WithMessage("TRANSACTION.RETURNING.ADD.NULL");
        }
    }
}
