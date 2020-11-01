using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Transaction.Models;

namespace Transaction.Service.Infrastructure
{

    public class OwnershipAuthorization<TKey, T> where T : class, IEntity<TKey>
    {
        private IRepository<TKey, T> _repo;

        private CurrentUserCredentialsGetter _credentialsGetter;


        public OwnershipAuthorization(IRepository<TKey, T> repo, CurrentUserCredentialsGetter credentialsGetter)
        {
            _repo = repo;
            _credentialsGetter = credentialsGetter;
        }

        public bool IsAuthorized(Expression<Func<T, bool>> identifiengEntityExpression, Expression<Func<T, User>> toUser)
        {
            var currentUser = _credentialsGetter.GetCuurentUser();
            if (currentUser is null) return false;

            var entity = _repo.Table.Where(identifiengEntityExpression).Select(toUser).Where(u => u.UserId == Guid.Parse(currentUser.UserId));
            if (entity.Any())
            {
                return true;
            }
            return false;
        }
    }

    public class EntityOwnership<TKey>
    {
        public TKey Key { get; set; }

        public User User { get; set; }
    }
}
