using System.Collections.Generic;

namespace Framework.Helpers
{
    public static class ICollectionHelpers
    {
        public static void AddRange<T>(this IList<T> source, IEnumerable<T> range)
        {
            foreach (var item in range)
                source.Add(item);
        }
    }
}
