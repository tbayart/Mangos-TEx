using System.Collections.Generic;

namespace WowApi.Models
{
    public class CharacterProgression
    {
        public List<Raid> Raids { get; set; }

        public class Raid
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Normal { get; set; }
            public int Heroic { get; set; }
            public List<Boss> Bosses { get; set; }
        }

        public class Boss
        {
            public int HeroicKills { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public int NormalKills { get; set; }
        }
    }
}
