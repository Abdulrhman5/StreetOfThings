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
                .WithMessage("TRANSACTION.OBJECT.RESERVE.NULL");
        }
    }
}
