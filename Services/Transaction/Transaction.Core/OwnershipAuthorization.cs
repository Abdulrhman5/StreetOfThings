using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Transaction.Core.Interfaces;
using Transaction.Domain;
using Transaction.Domain.Entities;

namespace Transaction.Core
{

    public class OwnershipAuthorization<TKey, T> : IOwnershipAuthorization<TKey, T>
        where T : BaseEntity<TKey> 
    {
        private IRepository<TKey, T> _repo;

        private IUserDataManager _userDataManager;


        public OwnershipAuthorization(IRepository<TKey, T> repo, IUserDataManager credentialsGetter)
        {
            _repo = repo;
            _userDataManager = credentialsGetter;
        }

        public bool IsAuthorized(Expression<Func<T, bool>> identifiengEntityExpression, Expression<Func<T, User>> toUser)
        {
            var currentUser = _userDataManager.GetCurrentUser();
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
