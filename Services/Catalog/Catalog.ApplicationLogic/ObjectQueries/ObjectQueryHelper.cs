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
            // if object is free
            (o.CurrentTransactionType == TransactionType.Free &&
            // if object has not been taken
            !o.ObjectFreeProperties.TakenAtUtc.HasValue) ||

            // is object lending?
            (o.CurrentTransactionType == TransactionType.Lending &&
            // is object valid?
            o.ObjectLoanProperties.ObjectLoans.All(ol => ol.LoanEndAt < DateTime.UtcNow));


        public Expression<Func<OfferedObject, bool>> IsValidObject => (o) => o.ObjectStatus == ObjectStatus.Available &&
        // if object has endDate and it is after now Or has not endDate
        (o.EndsAt > DateTime.UtcNow || !o.EndsAt.HasValue);
    }
}
