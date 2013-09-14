namespace WowApi.Models
{
    public class CharacterEquipement
    {
        public int AverageItemLevel { get; set; }
        public int AverageItemLevelEquipped { get; set; }
        public EquippedItem Back { get; set; }
        public EquippedItem Chest { get; set; }
        public EquippedItem Feet { get; set; }
        public EquippedItem Finger1 { get; set; }
        public EquippedItem Finger2 { get; set; }
        public EquippedItem Hands { get; set; }
        public EquippedItem Legs { get; set; }
        public EquippedItem MainHand { get; set; }
        public EquippedItem Neck { get; set; }
        public EquippedItem Shoulder { get; set; }
        public EquippedItem Trinket1 { get; set; }
        public EquippedItem Trinket2 { get; set; }
        public EquippedItem Waist { get; set; }
        public EquippedItem Wrist { get; set; }
    }
}
