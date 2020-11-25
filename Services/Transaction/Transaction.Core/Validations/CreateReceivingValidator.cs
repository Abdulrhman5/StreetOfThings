using FluentValidation;
using System;
using System.Threading.Tasks;
using Transaction.Core.Commands;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;
using System.Linq;
namespace Transaction.Core.Validations
{
    class CreateReceivingValidator : AbstractValidator<CreateReceivingCommand>
    {
        public CreateReceivingValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(receiving => receiving)
                .NotNull()
                .WithMessage("TRANSACTION.RECEIVING.ADD.NULL");

            RuleFor(receiving => receiving.RegistrationToken)
                .NotNull()
                .NotEmpty()
                .WithMessage("TRANSACTION.RECEIVING.ADD.NULL");
        }
    }
}
