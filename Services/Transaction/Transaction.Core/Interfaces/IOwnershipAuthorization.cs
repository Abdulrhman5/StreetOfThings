using System;
using System.Linq.Expressions;
using Transaction.Domain;
using Transaction.Domain.Entities;

namespace Transaction.Core.Interfaces
{
    public interface IOwnershipAuthorization<TKey, T> where T : class, IEntity<TKey>
    {
        bool IsAuthorized(Expression<Func<T, bool>> identifiengEntityExpression, Expression<Func<T, User>> toUser);
    }
}