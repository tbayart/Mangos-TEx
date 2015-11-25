using System;
using System.Linq;
using Framework.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WowApi.Helpers;
using WowApi.Models;

namespace WowApi.JsonConverters
{
    public class CharacterAchievementsConverter : JsonConverter
    {
        #region JsonConverter Members
        public override bool CanConvert(Type objectType)
        {
            return typeof(CharacterAchievements).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var output = new CharacterAchievements();
            JObject jobject = JObject.Load(reader);

            // process achievements
            JToken token = jobject["achievementsCompleted"];
            if (token != null)
            {
                output.Achievements = token.Values<int>().Select(o => new CharacterAchievements.Achievement { Id = o }).ToList();
                token = jobject["achievementsCompletedTimestamp"];
                if (token != null)
                {
                    var list = token.ToArray<long>();
                    output.Achievements.Fusion(list, (ach, o) => ach.Timestamp = o);
                }
            }

            // process criterias
            token = jobject["criteria"];
            if (token != null)
            {
                output.Criterias = token.Values<int>().Select(o => new CharacterAchievements.Criteria { Id = o }).ToList();
                token = jobject["criteriaCreated"];
                if (token != null)
                {
                    var list = token.ToArray<long>();
                    output.Criterias.Fusion(list, (crit, o) => crit.Created = o);
                }
                token = jobject["criteriaQuantity"];
                if (token != null)
                {
                    var list = token.ToArray<long>();
                    output.Criterias.Fusion(list, (crit, o) => crit.Quantity = o);
                }
                token = jobject["criteriaTimestamp"];
                if (token != null)
                {
                    var list = token.ToArray<long>();
                    output.Criterias.Fusion(list, (crit, o) => crit.Timestamp = o);
                }
            }

            return output;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
        #endregion JsonConverter Members
    }
}
