using System;
using System.Collections.Generic;

namespace Framework.Helpers
{
    public static class IListHelpers
    {
        public static void Fusion<T1, T2>(this IList<T1> inout, IList<T2> aggregate, Action<T1, T2> action)
        {
            for (int index = 0; index < inout.Count; ++index)
                action(inout[index], aggregate[index]);
        }
    }
}
