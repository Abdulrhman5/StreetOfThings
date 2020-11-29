using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Transaction.Core.Interfaces;

namespace Transaction.Infrastructure.Data
{
    class QueryableHelper : IQueryableHelper
    {
        public IQueryable<TEntity> Include<TEntity>(IQueryable<TEntity> source, string navigationPropertyPath) where TEntity : class
        {
            return source.Include(navigationPropertyPath);
        }

        public IQueryable<TEntity> Include<TEntity, TProperty>(IQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> navigationPropertyPath) where TEntity : class
        {
            return source.Include(navigationPropertyPath);
        }

        public async Task<List<T>> ToListAsync<T>(IQueryable<T> query)
        {
            return await query.ToListAsync();
        }
    }
}
