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
    }
}
