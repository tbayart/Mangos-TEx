using System.Collections.Generic;

namespace WowApi.Models
{
    public class CharacterAchievements
    {
        public class Achievement
        {
            public int Id { get; set; }
            public long Timestamp { get; set; }
        }

        public class Criteria
        {
            public int Id { get; set; }
            public long Created { get; set; }
            public int Quantity { get; set; }
            public long Timestamp { get; set; }
        }

        public List<Achievement> Achievements { get; set; }
        public List<Criteria> Criterias { get; set; }
    }
}
