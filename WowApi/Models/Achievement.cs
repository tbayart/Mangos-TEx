using System.Collections.Generic;
using Newtonsoft.Json;

namespace WowApi.Models
{
    public class Achievement
    {
        public bool AccountWide { get; set; }
        [JsonProperty("criteria")]
        public List<Criteria> CriteriaList { get; set; }
        public string Description { get; set; }
        public int FactionId { get; set; }
        public string Icon { get; set; }
        public int Id { get; set; }
        public int Points { get; set; }
        public string Reward { get; set; }
        public List<ItemBase> RewardItems { get; set; }
        public string Title { get; set; }

        public class Criteria
        {
            public string Description { get; set; }
            public int Id { get; set; }
            public int Max { get; set; }
            public int OrderIndex { get; set; }
        }
    }
}
