using System.Collections.Generic;

namespace SecretPlan.Core
{
    public static class ListExtensions
    {
        public static bool IsValidIndex<T>(this IList<T> collection, int i)
        {
            return i < collection.Count && i >= 0;
        }
    }
}