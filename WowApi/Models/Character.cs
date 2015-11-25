using System.Collections.Generic;
using Newtonsoft.Json;
using WowApi.JsonConverters;

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

        [JsonConverter(typeof(CharacterAchievementsConverter))]
        public CharacterAchievements Achievements { get; set; }

        public CharacterAppearance Appearance { get; set; }

        [JsonConverter(typeof(CharacterFeedConverter))]
        public List<CharacterFeed> Feed { get; set; }

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

        [JsonConverter(typeof(CharacterTalentsConverter))]
        public List<CharacterTalent> Talents { get; set; }

        public List<CharacterTitle> Titles { get; set; }
    }
}
