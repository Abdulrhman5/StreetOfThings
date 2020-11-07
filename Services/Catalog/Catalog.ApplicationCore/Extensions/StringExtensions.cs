using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationCore.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Equals Ignoring the case
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool EqualsIC(this string value, string value2)
        {
            return value.Equals(value2, StringComparison.OrdinalIgnoreCase);
        }
    }
}
