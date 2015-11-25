using System.Collections.Generic;
using Newtonsoft.Json;
namespace WowApi.Models
{
    public class CharacterEquipement
    {
        public int AverageItemLevel { get; set; }
        public int AverageItemLevelEquipped { get; set; }
        public ItemBase Back { get; set; }
        public ItemBase Chest { get; set; }
        public ItemBase Feet { get; set; }
        public ItemBase Finger1 { get; set; }
        public ItemBase Finger2 { get; set; }
        public ItemBase Hands { get; set; }
        public ItemBase Legs { get; set; }
        public ItemBase MainHand { get; set; }
        public ItemBase Neck { get; set; }
        public ItemBase Shoulder { get; set; }
        public ItemBase Trinket1 { get; set; }
        public ItemBase Trinket2 { get; set; }
        public ItemBase Waist { get; set; }
        public ItemBase Wrist { get; set; }
    }
}
