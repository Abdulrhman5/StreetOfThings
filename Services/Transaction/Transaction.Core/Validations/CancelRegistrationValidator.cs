using FluentValidation;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Transaction.Core.Commands;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;

namespace Transaction.Core.Validations
{
    class CancelRegistrationValidator : AbstractValidator<CancelRegistrationCommand>
    {

        public CancelRegistrationValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(cancelRegistration => cancelRegistration)
                .NotNull()
                .WithMessage("TRANSACTION.REGISTRATION.CANCEL.NULL");

            RuleFor(cancelRegistration => cancelRegistration.RegistrationId)
                .Must(registrationId => Guid.TryParse(registrationId, out var result))
                .WithMessage("TRANSACTION.REGISTRATION.INVALID.DATA");
        }
    }
}
