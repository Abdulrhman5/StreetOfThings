using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Transaction.Core.Interfaces;
using Transaction.Domain;

namespace Transaction.Core.Extensions
{
    public static class ListExtensions
    {
        public static bool IsNullOrEmpty<T>(this List<T> items)
        {
            if (items is null || !items.Any()) return true;
            return false;
        }

        public static bool HasItems<T>(this List<T> items) => !IsNullOrEmpty(items);

        public static List<T> SkipTake<T>(this IQueryable<T> query, PagingArguments arguments)
        {
            return query.Skip(arguments.StartObject).Take(arguments.Size).ToList();
        }   
        
        public static async Task<List<T>> SkipTakeAsync<T>(this IQueryable<T> query, IQueryableHelper queryHelper, PagingArguments arguments) 
        {
            return await queryHelper.ToListAsync(query.Skip(arguments.StartObject).Take(arguments.Size));
        } 
        
        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> query, IQueryableHelper queryHelper) 
        {
            return await queryHelper.ToListAsync(query);
        }

        public static IQueryable<TEntity> Include<TEntity>(this IQueryable<TEntity> source,
            string navigationPropertyPath,
            IQueryableHelper queryHelper) where TEntity : class
        {
            return queryHelper.Include(source, navigationPropertyPath);
        }

        public static IQueryable<TEntity> Include<TEntity, TProperty>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, TProperty>> navigationPropertyPath,
            IQueryableHelper queryHelper) 
            where TEntity : class
        {
            return queryHelper.Include(source, navigationPropertyPath);
        }
    }
}
