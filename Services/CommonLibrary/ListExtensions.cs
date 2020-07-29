using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CommonLibrary
{
    public static class ListExtensions
    {
        public static List<T> SkipTake<T>(this IQueryable<T> query, PagingArguments arguments)
        {
            return query.Skip(arguments.StartObject).Take(arguments.Size).ToList();
        }

        public static async  Task<List<T>> SkipTakeAsync<T>(this IQueryable<T> query, PagingArguments arguments)
        {
            return await query.Skip(arguments.StartObject).Take(arguments.Size).ToListAsync();
        }
    }
}
