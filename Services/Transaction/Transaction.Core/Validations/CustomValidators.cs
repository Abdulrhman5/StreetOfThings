using FluentValidation;
using System;
using System.Linq.Expressions;
using Transaction.Core.Interfaces;
using Transaction.Domain;
using Transaction.Domain.Entities;

namespace Transaction.Core.Validations
{
    public static class CustomValidators
    {
        public static IRuleBuilderOptions<T, T> MustBeAuthorized<T>(this IRuleBuilder<T, T> ruleBuilder, IUserDataManager userDataManager)
        {
            return ruleBuilder.Must(t => userDataManager.GetCurrentUser() is object);
        }

        public static IRuleBuilderOptions<T, T> MustBeAuthorized<T, TKey, TEntity>(this IRuleBuilder<T, T> ruleBuilder,
            IOwnershipAuthorization<TKey, TEntity> ownerShipAuthorizer,
            Func<T, Expression<Func<TEntity, bool>>> identifingEntityExpression,
            Func<T, Expression<Func<TEntity, User>>> navigationToUser)
            where TEntity : class, IEntity<TKey>
        {
            return ruleBuilder.Must(t => ownerShipAuthorizer.IsAuthorized(identifingEntityExpression(t), navigationToUser(t)));
        }
    }
}
