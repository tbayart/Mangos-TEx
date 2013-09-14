using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace WowApi.Models
{
    public class Character
    {
        public string Name { get; set; }
        public string Realm { get; set; }
        public int Class { get; set; }
        public int Race { get; set; }
        public int Gender { get; set; }
        public int Level { get; set; }
        public int AchievementPoints { get; set; }
        public string Battlegroup { get; set; }
        public string CalcClass { get; set; }
        public long LastModified { get; set; }
        public string Thumbnail { get; set; }

        public CharacterAchievements Achievements { get; set; }

        public CharacterAppearance Appearance { get; set; }

        //public List<CharacterFeed> Feed { get; set; }

        public CharacterGuild Guild { get; set; }

        public List<CharacterHunterPet> HunterPets { get; set; }

        [JsonProperty("items")]
        public CharacterEquipement Equipment { get; set; }

        public CharacterMounts Mounts { get; set; }

        public CharacterPets Pets { get; set; }

        public List<CharacterPetSlot> PetSlots { get; set; }

        public CharacterProfessions Professions { get; set; }

        public CharacterProgression Progression { get; set; }

        public CharacterPvp Pvp { get; set; }

        public List<int> Quests { get; set; }

        public List<CharacterReputation> Reputation { get; set; }

        public CharacterStats Stats { get; set; }

        public List<CharacterTalent> Talents { get; set; }

        public List<CharacterTitle> Titles { get; set; }
    }

    // feed
    public abstract class CharacterFeed
    {
        public string TYPE { get; set; }
        public long Timestamp { get; set; }
    }

    // Unprocessed Feed Type
    public class FeedObject : CharacterFeed
    {
        public string Data { get; set; }
    }

    // Type "ACHIEVEMENT"
    public class FeedAchievement : CharacterFeed
    {
        public Achievement Achievement { get; set; }
        public bool FeatOfStrength { get; set; }
    }
    public class Achievement
    {
        public bool AccountWide { get; set; }
        public List<Criteria> Criteria { get; set; }
        public string Description { get; set; }
        public int FactionId { get; set; }
        public string Icon { get; set; }
        public int Id { get; set; }
        public int Points { get; set; }
#warning missing
        //RewardItems": [], 
        public string Title { get; set; }
    }
    public class Criteria
    {
        public string Description { get; set; }
        public int Id { get; set; }
        public int Max { get; set; }
        public int OrderIndex { get; set; }
    }

    // Type "LOOT"
    public class FeedLoot : CharacterFeed
    {
        public int ItemId { get; set; }
    }

    // Type "BOSSKILL"
    public class FeedBosskill : CharacterFeed
    {
        public Achievement Achievement { get; set; }
        public Criteria Criteria { get; set; }
        public bool FeatOfStrength { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
}
