using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Helpers
{
    public static class IEnumerableHelpers
    {
        public static void ForEach(this IEnumerable source, Action<object> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }
    }
}
