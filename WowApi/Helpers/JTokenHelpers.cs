using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace WowApi.Helpers
{
    public static class JTokenHelpers
    {
        public static T[] ToArray<T>(this JToken token)
        {
            return token.Values<T>().ToArray();
        }

        public static List<T> ToList<T>(this JToken token)
        {
            return token.Values<T>().ToList();
        }

        public static void Fusion<T1, T2>(this IList<T1> inout, IList<T2> aggregate, Action<T1, T2> action)
        {
            for (int index = 0; index < inout.Count; ++index)
                action(inout[index], aggregate[index]);
        }
    }
}
