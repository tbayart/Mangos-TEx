using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace WowApi.Models
{
    public abstract class CharacterFeed
    {
        public static Type GetType(string jsonType)
        {
            switch (jsonType)
            {
                case "ACHIEVEMENT": return typeof(FeedAchievement);
                case "BOSSKILL": return typeof(FeedBosskill);
                case "CRITERIA": return typeof(FeedCriteria);
                case "LOOT": return typeof(FeedLoot);
            }
            return null;
        }

        public string Type {get;set;}
        public long Timestamp { get; set; }
    }

    // Type "ACHIEVEMENT"
    public class FeedAchievement : CharacterFeed
    {
        public new const string Type = "ACHIEVEMENT";
        public Achievement Achievement { get; set; }
        public bool FeatOfStrength { get; set; }
    }

    // Type "BOSSKILL"
    public class FeedBosskill : CharacterFeed
    {
        public new const string Type = "BOSSKILL";
        public Achievement Achievement { get; set; }
        public Criteria Criteria { get; set; }
        public bool FeatOfStrength { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }

    // Type "CRITERIA"
    public class FeedCriteria : CharacterFeed
    {
        public new const string Type = "CRITERIA";
        public Achievement Achievement { get; set; }
        public bool FeatOfStrength { get; set; }
        public Criteria Criteria { get; set; }
    }

    // Type "LOOT"
    public class FeedLoot : CharacterFeed
    {
        public new const string Type = "LOOT";
        public int ItemId { get; set; }
    }
}
