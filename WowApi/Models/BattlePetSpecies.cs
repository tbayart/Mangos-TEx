using System.Collections.Generic;

namespace WowApi.Models
{
    public class BattlePetSpecies
    {
        public List<Ability> Abilities { get; set; }
        public bool CanBattle { get; set; }
        public int CreatureId { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int PetTypeId { get; set; }
        public string Source { get; set; }
        public int SpeciesId { get; set; }

        public class Ability : BattlePetAbility
        {
            public int Order { get; set; }
            public int RequiredLevel { get; set; }
            public int Slot { get; set; }
        }
    }
}
