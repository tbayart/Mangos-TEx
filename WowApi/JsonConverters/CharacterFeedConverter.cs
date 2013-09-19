using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WowApi.Models;

namespace WowApi.JsonConverters
{
    public class CharacterFeedConverter : JsonConverter
    {
        #region JsonConverter Members
        public override bool CanConvert(Type objectType)
        {
            return typeof(IEnumerable<CharacterFeed>).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray jarray = JArray.Load(reader);

            var output = new List<CharacterFeed>(jarray.Count);
            foreach (JToken token in jarray)
            {
                string jsonType = token["type"].Value<string>();
                Type type = CharacterFeed.GetType(jsonType);
                if (type != null)
                {
                    string data = token.ToString();
                    CharacterFeed value = (CharacterFeed)JsonConvert.DeserializeObject(data, type);
                    output.Add(value);
                }
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
