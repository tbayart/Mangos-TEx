using System.Collections.Generic;

namespace WowApi.Models
{
    public class CharacterPets
    {
        public List<CharacterPet> Collected { get; set; }
        public int NumCollected { get; set; }
        public int NumNotCollected { get; set; }
    }
}
