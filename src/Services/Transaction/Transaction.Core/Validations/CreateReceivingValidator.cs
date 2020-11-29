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
                .WithErrorMessage(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.NULL",
                    Message = "Please send valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });

            RuleFor(receiving => receiving.RegistrationToken)
                .NotNull()
                .NotEmpty()
                .WithErrorMessage(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.NULL",
                    Message = "Please send valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });
        }
    }
}
