using System.Collections.Generic;
using Newtonsoft.Json;

namespace WowApi.Models
{
    public class Item
    {
        public string Description { get; set; }
        public string Icon { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quality { get; set; }
        [JsonIgnore]
        public Dictionary<string, int> TooltipParams { get; set; }
    }
}
