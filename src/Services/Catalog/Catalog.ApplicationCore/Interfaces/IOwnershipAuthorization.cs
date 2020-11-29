using Catalog.ApplicationCore.Entities;
using System;
using System.Linq.Expressions;

namespace Catalog.ApplicationCore.Interfaces
{
    public interface IOwnershipAuthorization<TKey, T> where T : class, IEntity<TKey>
    {
        bool IsAuthorized(Expression<Func<T, bool>> identifiengEntityExpression, Expression<Func<T, User>> toUser);
    }
}