using System.Collections.Generic;

namespace WowApi.Models
{
    public class RatedBattlegrounds
    {
        public List<Battleground> Battlegrounds { get; set; }
        public int PersonalRating { get; set; }
    }
}
