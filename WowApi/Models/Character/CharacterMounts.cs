using System.Collections.Generic;

namespace WowApi.Models
{
    public class CharacterMounts
    {
        public List<CharacterMount> Collected { get; set; }
        public int NumCollected { get; set; }
        public int NumNotCollected { get; set; }
    }
}
