using System.Collections.Generic;
using Newtonsoft.Json;

namespace WowApi.Models
{
    public abstract class AchievementNode
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public class AchievementsNode : AchievementNode
        {
            public List<Achievement> Achievements { get; set; }
        }

        public class CategoriesNode : AchievementNode
        {
            public List<AchievementsNode> Categories { get; set; }
        }
    }
}
