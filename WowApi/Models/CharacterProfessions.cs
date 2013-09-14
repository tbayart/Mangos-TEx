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
    }
}
