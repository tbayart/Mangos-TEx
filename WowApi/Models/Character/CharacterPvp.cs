using System.Collections.Generic;
using Newtonsoft.Json;

namespace WowApi.Models
{
    public class CharacterPvp
    {
#warning missing
        //public List<> ArenaTeams { get; set; }

        [JsonProperty("ratedBattlegrounds")]
        public RatedBattlegrounds Battlegrounds { get; set; }
        public int TotalHonorableKills { get; set; }

        public class RatedBattlegrounds
        {
            public List<Battleground> Battlegrounds { get; set; }
            public int PersonalRating { get; set; }
        }

        public class Battleground
        {
            public string Name { get; set; }
            public int Played { get; set; }
            public int Won { get; set; }
        }
    }
}
