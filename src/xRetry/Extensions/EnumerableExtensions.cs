using System;
using System.Collections.Generic;
using System.Linq;

namespace xRetry.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool ContainsAny<T>(this IEnumerable<T> values, T[] searchFor, IEqualityComparer<T> comparer = null)
        {
            if (searchFor == null)
            {
                throw new ArgumentNullException(nameof(searchFor));
            }
            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            return searchFor.Length != 0 && 
                   values.Any(val => searchFor.Any(search => comparer.Equals(val, search)));
        }
    }
}
