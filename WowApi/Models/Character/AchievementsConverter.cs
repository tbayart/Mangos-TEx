using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WowApi.Models
{
    public class AchievementsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(CharacterFeed).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new TupleList<int, long>();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
