using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WowApi.JsonConverters
{
    public class ContainerConverter<T> : JsonConverter
    {
        #region Fields
        private string _containerName;
        #endregion Fields

        #region Ctor
        public ContainerConverter(string containerName)
        {
            _containerName = containerName;
        }
        #endregion Ctor

        #region JsonConverter Members
        public override bool CanConvert(Type objectType)
        {
            return typeof(IEnumerable<T>).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jdata = JObject.Load(reader)[_containerName];
            if (jdata == null)
                return null;

            List<T> output = new List<T>();
            foreach (JToken token in jdata)
            {
                string data = token.ToString();
                T value = (T)JsonConvert.DeserializeObject(data, typeof(T));
                output.Add(value);
            }

            return output.Count > 0
                ? output
                : null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
        #endregion JsonConverter Members
    }
}
