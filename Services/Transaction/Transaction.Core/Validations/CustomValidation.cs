using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Core.Validations
{
    public static class CustomValidation
    {
        public static IRuleBuilderOptions<T, TProperty> WithErrorMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, ErrorMessage errorMessage)
        {
            rule.WithMessage<T, TProperty>(errorMessage.Message)
                .WithErrorCode<T,TProperty>(errorMessage.ErrorCode)
                .WithState(x => errorMessage);
            return rule;
        }
    }
}
