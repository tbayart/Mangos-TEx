using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WowApi.Models;

namespace WowApi.JsonConverters
{
    public class TalentsConverter : JsonConverter
    {
        #region JsonConverter Members
        public override bool CanConvert(Type objectType)
        {
            return typeof(IEnumerable<Talent>).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jdata = JObject.Load(reader);
            if (jdata == null)
                return null;

            List<Talent> output = new List<Talent>(jdata.Count());
            foreach (JToken token in jdata)
            {
                JToken jtalent = token.First();
                string data = jtalent.ToString();
                Talent value = (Talent)JsonConvert.DeserializeObject<Talent>(data);
                // "talents" token is tagged as ignore because it needs a SelectMany step to flatten 2 dimensions array
                value.Talents = jtalent["talents"]
                    .SelectMany(tier => tier)
                    .Select(tier => (TalentTier)JsonConvert.DeserializeObject<TalentTier>(tier.ToString()))
                    .ToList();
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
