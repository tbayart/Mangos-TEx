using System.Collections.Generic;
using Newtonsoft.Json;

namespace WowApi.Models
{
    public class ItemBase
    {
        public string Description { get; set; }
        public string Icon { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quality { get; set; }
        [JsonIgnore]
        public Dictionary<string, int> TooltipParams { get; set; }
    }

    public class Item : ItemBase
    {
        public int Armor { get; set; }
        public int BaseArmor { get; set; }
        public List<BonusStat> BonusStats { get; set; }
        public int BuyPrice { get; set; }
        public int ContainerSlots { get; set; }
        public int DisenchantingSkillRank { get; set; }
        public int DisplayInfoId { get; set; }
        public bool Equippable { get; set; }
        public bool HasSockets { get; set; }
        public bool HeroicTooltip { get; set; }
        public int InventoryType { get; set; }
        public bool IsAuctionable { get; set; }
        public int ItemBind { get; set; }
        public int ItemClass { get; set; }
        public int ItemLevel { get; set; }
        public Source ItemSource { get; set; }
        //"itemSpells": [], 
        public int ItemSubClass { get; set; }
        public int MaxCount { get; set; }
        public int MaxDurability { get; set; }
        public int MinFactionId { get; set; }
        public int MinReputation { get; set; }
        public string NameDescription { get; set; }
        public string NameDescriptionColor { get; set; }
        public int RequiredLevel { get; set; }
        public int RequiredSkill { get; set; }
        public int RequiredSkillRank { get; set; }
        public int SellPrice { get; set; }
        public int Stackable { get; set; }
        public bool Upgradable { get; set; }
        public WeaponInfos WeaponInfo { get; set; }

        public class BonusStat
        {
            public int Amount { get; set; }
            public int Stat { get; set; }
        }

        public class Source
        {
            public int SourceId { get; set; }
            public string SourceType { get; set; }
        }

        public class WeaponInfos
        {
            public Damage Damage { get; set; }
            public double Dps { get; set; }
            public double WeaponSpeed { get; set; }
        }

        public class Damage
        {
            public double ExactMax { get; set; }
            public double ExactMin { get; set; }
            public int Max { get; set; }
            public int Min { get; set; }
        }
    }
}
