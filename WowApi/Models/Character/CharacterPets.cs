using WowApi.Models.Generic;

namespace WowApi.Models
{
    public class CharacterPets : CharacterCreatures<CharacterPets.Pet>
    {
        public class Pet
        {
            public int BattlePetId { get; set; }
            public bool CanBattle { get; set; }
            public int CreatureId { get; set; }
            public string CreatureName { get; set; }
            public string Icon { get; set; }
            public bool IsFavorite { get; set; }
            public int ItemId { get; set; }
            public string Name { get; set; }
            public int QualityId { get; set; }
            public int SpellId { get; set; }
            public PetStats Stats { get; set; }
        }

        public class PetStats
        {
            public int BreedId { get; set; }
            public int Health { get; set; }
            public int Level { get; set; }
            public int PetQualityId { get; set; }
            public int Power { get; set; }
            public int SpeciesId { get; set; }
            public int Speed { get; set; }
        }
    }
}
