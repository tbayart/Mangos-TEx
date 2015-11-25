using System.Collections.Generic;
using Newtonsoft.Json;

namespace WowApi.Models
{
    public class CharacterProfessions
    {
        [JsonProperty("primary")]
        List<Profession> Primary { get; set; }
        [JsonProperty("secondary")]
        List<Profession> Secondary { get; set; }

        public class Profession
        {
            public string Icon { get; set; }
            public int Id { get; set; }
            public int Max { get; set; }
            public string Name { get; set; }
            public int Rank { get; set; }
            public List<int> Recipes { get; set; }
        }
    }
}
