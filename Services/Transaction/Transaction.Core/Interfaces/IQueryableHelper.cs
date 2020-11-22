using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transaction.Core.Interfaces
{
    public interface IQueryableHelper
    {
        public Task<List<T>> ToListAsync<T>(IQueryable<T> query);

        public IQueryable<TEntity> Include<TEntity>(IQueryable<TEntity> source, string navigationPropertyPath) where TEntity : class;
    }
}
