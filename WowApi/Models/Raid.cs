using System.Collections.Generic;

namespace WowApi.Models
{
    public class Raid
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Normal { get; set; }
        public int Heroic { get; set; }
        public List<Boss> Bosses { get; set; }
    }
}
