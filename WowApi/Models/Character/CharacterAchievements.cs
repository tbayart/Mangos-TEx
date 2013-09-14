using System.Collections.Generic;
using Newtonsoft.Json;

namespace WowApi.Models
{
    public class CharacterAchievements
    {
        public List<int> AchievementsCompleted { get; set; }
        public List<long> AchievementsCompletedTimestamp { get; set; }
        public List<int> Criteria { get; set; }
        public List<long> CriteriaCreated { get; set; }
        public List<int> CriteriaQuantity { get; set; }
        public List<long> CriteriaTimestamp { get; set; }
    }
}
