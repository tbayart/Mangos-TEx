using System.Collections.Generic;

namespace WowApi.Models
{
    public class Achievement
    {
        public bool AccountWide { get; set; }
        public List<Criteria> Criteria { get; set; }
        public string Description { get; set; }
        public int FactionId { get; set; }
        public string Icon { get; set; }
        public int Id { get; set; }
        public int Points { get; set; }
        public string Reward { get; set; }
        public List<Item> RewardItems { get; set; }
        public string Title { get; set; }
    }
}
