using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
