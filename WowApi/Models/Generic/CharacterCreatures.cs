using System.Collections.Generic;

namespace WowApi.Models.Generic
{
    public class CharacterCreatures<T>
    {
        public List<T> Collected { get; set; }
        public int NumCollected { get; set; }
        public int NumNotCollected { get; set; }
    }
}
