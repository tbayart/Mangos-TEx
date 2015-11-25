namespace WowApi.Models
{
    public class CharacterGuild
    {
        public int AchievementPoints { get; set; }
        public string Battlegroup { get; set; }
        public GuildEmblem Emblem { get; set; }
        public int Level { get; set; }
        public int Members { get; set; }
        public string Name { get; set; }
        public string Realm { get; set; }
    }

    public class GuildEmblem
    {
        public string BackgroundColor { get; set; }
        public int Border { get; set; }
        public string BorderColor { get; set; }
        public int Icon { get; set; }
        public string IconColor { get; set; }
    }
}
