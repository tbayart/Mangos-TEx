using System.Collections.Generic;

namespace WowApi.Models
{
    public class CharacterPetSlot
    {
        public List<int> Abilities { get; set; }
        public int BattlePetId { get; set; }
        public bool IsEmpty { get; set; }
        public bool IsLocked { get; set; }
        public int Slot { get; set; }
    }
}
