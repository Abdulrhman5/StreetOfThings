using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary
{
    public static class ListHelpers
    {
        public static bool IsNullOrEmpty<T>(this List<T> items)
        {
            if (items is null || !items.Any()) return true;
            return false;
        }

        public static bool HasItems<T>(this List<T> items) => !IsNullOrEmpty(items);
    }
}
