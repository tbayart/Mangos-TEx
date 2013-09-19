using System;
using System.Collections.Generic;
using System.Linq;
using WowApi.Models;

namespace WowApi
{
    /// <summary>
    /// regisrty of WoW API data types
    /// </summary>
    internal class WowApiDataType
    {
        #region Internal
        public class WowApiDataTypes : Dictionary<Type, WowApiDataType>
        {
            public void Add(Type type, string name, string path) { Add(type, new WowApiDataType(type, name, path)); }
        }

        internal static WowApiDataTypes _dataTypes = new WowApiDataTypes
        {
            { typeof(Achievement),          "Achievement details",      "achievement/{0}"             },
            //auction/data/" + realm
            { typeof(BattlePetAbility),     "Battle pet ability",        "battlePet/ability/{0}"       },
            { typeof(BattlePetSpecies),     "Battle pet species",        "battlePet/species/{0}"       },
            { typeof(BattlePetStats),       "Battle pet stats",          "battlePet/stats/{0}?level={1}&breedId={2}&qualityId={3}" },
            //challenge/" + Realm
            //challenge/region
            { typeof(Character),            "Character profile",        "character/{0}/{1}{2}"        },
            { typeof(Item),                 "Item details",             "item/{0}"                    },
            { typeof(ItemSet),              "Item set",                 "item/set/{0}"                },
            //guild/" + Realm + "/" + GuildName
            //arena/" + Realm + "/" + TeamSize + "/" + TeamName
            //pvp/arena/" + Battlegroup + "/" + TeamSize
            //pvp/ratedbg/ladder
            { typeof(Quest),                "Quest details",            "quest/{0}"                   },
            //realm/status
            { typeof(Recipe),               "Recipe details",           "recipe/{0}"                  },
            { typeof(Spell),                "Spell details",            "spell/{0}"                   },
            //data/battlegroups/
            { typeof(CharacterRace),        "Character races",          "data/character/races"        },
            { typeof(CharacterClass),       "Character classes",        "data/character/classes"      },
            { typeof(AchievementNode),      "Achievements",             "data/character/achievements" },
            //data/guild/rewards
            //data/guild/perks
            //data/guild/achievements
            { typeof(ItemClass),            "Item classes",             "data/item/classes"           },
            { typeof(Talent),               "Talents",                  "data/talents"                },
            { typeof(PetType),              "Pet types",                "data/pet/types"              },
        };
        #endregion Internal

        #region Ctor
        private WowApiDataType(Type type, string name, string path)
        {
            Type = type;
            Name = name;
            Path = path;
        }
        #endregion Ctor

        #region Properties
        public Type Type { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        #endregion Properties

        #region Static Properties
        // resources by id
        public static WowApiDataType Achievement { get { return _dataTypes[typeof(Achievement)]; } }
        public static WowApiDataType BattlePetAbility { get { return _dataTypes[typeof(BattlePetAbility)]; } }
        public static WowApiDataType BattlePetSpecies { get { return _dataTypes[typeof(BattlePetSpecies)]; } }
        public static WowApiDataType BattlePetStats { get { return _dataTypes[typeof(BattlePetStats)]; } }
        public static WowApiDataType Character { get { return _dataTypes[typeof(Character)]; } }
        public static WowApiDataType Item { get { return _dataTypes[typeof(Item)]; } }
        public static WowApiDataType ItemSet { get { return _dataTypes[typeof(ItemSet)]; } }
        public static WowApiDataType Quest { get { return _dataTypes[typeof(Quest)]; } }
        public static WowApiDataType Recipe { get { return _dataTypes[typeof(Recipe)]; } }
        public static WowApiDataType Spell { get { return _dataTypes[typeof(Spell)]; } }
        // global resources
        public static WowApiDataType CharacterRaces { get { return _dataTypes[typeof(CharacterRace)]; } }
        public static WowApiDataType CharacterClasses { get { return _dataTypes[typeof(CharacterClass)]; } }
        public static WowApiDataType Achievements { get { return _dataTypes[typeof(AchievementNode)]; } }
        public static WowApiDataType ItemClasses { get { return _dataTypes[typeof(ItemClass)]; } }
        public static WowApiDataType Talents { get { return _dataTypes[typeof(Talent)]; } }
        public static WowApiDataType PetTypes { get { return _dataTypes[typeof(PetType)]; } }

        public static WowApiDataType[] AllDataTypes { get { return _dataTypes.Values.ToArray(); } }
        #endregion Static Properties
    }
}
