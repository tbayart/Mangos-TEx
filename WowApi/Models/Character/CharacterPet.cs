namespace WowApi.Models
{
    public class CharacterPet
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
        public CharacterPetStats Stats { get; set; }
    }
}
