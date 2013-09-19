using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WowApi.Models;

namespace WowApi.JsonConverters
{
    public class CharacterTalentsConverter : JsonConverter
    {
        #region JsonConverter Members
        public override bool CanConvert(Type objectType)
        {
            return typeof(IEnumerable<CharacterTalent>).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray jarray = JArray.Load(reader);
            if (jarray == null)
                return null;

            var output = new List<CharacterTalent>(jarray.Count);
            foreach (JToken token in jarray)
            {
                string data = token.ToString();
                CharacterTalent value = (CharacterTalent)JsonConvert.DeserializeObject<CharacterTalent>(data);
                JToken glyphs = token["glyphs"];
                data = glyphs["minor"].ToString();
                value.MinorGlyphs = (List<TalentGlyph>)JsonConvert.DeserializeObject<List<TalentGlyph>>(data);
                data = glyphs["major"].ToString();
                value.MajorGlyphs = (List<TalentGlyph>)JsonConvert.DeserializeObject<List<TalentGlyph>>(data);
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
