using Catalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Catalog.ApplicationLogic.ObjectQueries
{
    public class ObjectQueryHelper
    {
        public Expression<Func<OfferedObject, bool>> ValidForFreeAndLendibg => (o) =>
            // The object is actually is at the owner
            o.Transactions.All(t => t.ReturnId is object);


        public Expression<Func<OfferedObject, bool>> IsValidObject => (o) =>
        o.ObjectStatus == ObjectStatus.Available &&
        // if object has endDate and it is after now Or has not endDate
        (o.EndsAt > DateTime.UtcNow || !o.EndsAt.HasValue) &&
        // The owner is available
        o.OwnerLogin.User.Status == UserStatus.Available;

    }
}
