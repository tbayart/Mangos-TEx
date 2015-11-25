using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WowApi.Models;

namespace WowApi.JsonConverters
{
    public class AchievementsConverter : JsonConverter
    {
        #region JsonConverter Members
        public override bool CanConvert(Type objectType)
        {
            return typeof(IEnumerable<AchievementNode>).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jdata = JObject.Load(reader)["achievements"];
            if (jdata == null)
                return null;

            List<AchievementNode> output = new List<AchievementNode>(jdata.Count());
            Type nodeType = null;
            foreach (JToken token in jdata)
            {
                if (token["categories"] != null)
                    nodeType = typeof(AchievementNode.CategoriesNode);
                else if (token["achievements"] != null)
                    nodeType = typeof(AchievementNode.AchievementsNode);
                else
                    continue;
                string data = token.ToString();
                AchievementNode value = (AchievementNode)JsonConvert.DeserializeObject(data, nodeType);
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
