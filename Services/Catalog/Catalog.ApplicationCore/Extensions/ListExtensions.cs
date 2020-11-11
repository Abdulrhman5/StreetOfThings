using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Interfaces;
using Catalog.ApplicationCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Extensions
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
        
        public static async Task<List<T>> SkipTakeAsync<T,TKey,TEntity>(this IQueryable<T> query, IRepository<TKey,TEntity> repo, PagingArguments arguments) 
            where TEntity : class, IEntity<TKey>
        {
            return await repo.ToListAsync(query.Skip(arguments.StartObject).Take(arguments.Size));
        }

    }
}
