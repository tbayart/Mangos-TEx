namespace WowApi.Models
{
    public class CharacterMount
    {
        public int CreatureId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool IsAquatic { get; set; }
        public bool IsFlying { get; set; }
        public bool IsGround { get; set; }
        public bool IsJumping { get; set; }
        public int ItemId { get; set; }
        public int QualityId { get; set; }
        public int SpellId { get; set; }
    }
}
