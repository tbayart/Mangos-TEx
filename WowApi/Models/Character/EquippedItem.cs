using System.Collections.Generic;

namespace WowApi.Models
{
    public class EquippedItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quality { get; set; }
        public string Icon { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<string, int> TooltipParams { get; set; }
    }
}
