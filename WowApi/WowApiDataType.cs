using System.Collections.Generic;

namespace WowApi
{
    public class WowApiDataType
    {
        #region Internal
        internal enum WowApiDataKey
        {
            c_races,
            c_class,
            c_achiev,
            i_class,
            talents,
            p_types,
            achiev_id,
            item_id,
            quest_id,
            recipe_id,
            spell_id
        }

        internal class WowApiDataTypes : Dictionary<WowApiDataKey, WowApiDataType>
        {
            public void Add(WowApiDataKey key, string name, string path) { Add(key, new WowApiDataType(name, path)); }
        }

        internal static WowApiDataTypes _types = new WowApiDataTypes
        {
            { WowApiDataKey.c_races,   "Character Races",        "data/character/races"        },
            { WowApiDataKey.c_class,   "Character Classes",      "data/character/classes"      },
            { WowApiDataKey.c_achiev,  "Character Achievements", "data/character/achievements" },
            { WowApiDataKey.i_class,   "Item Classes",           "data/item/classes"           },
            { WowApiDataKey.talents,   "Talents",                "data/talents"                },
            { WowApiDataKey.p_types,   "Pet Types",              "data/pet/types"              },
            { WowApiDataKey.achiev_id, "Achievement details",    "achievement/"                },
            { WowApiDataKey.item_id,   "Item details",           "item/"                       },
            { WowApiDataKey.quest_id,  "Quest details",          "quest/"                      },
            { WowApiDataKey.recipe_id, "Recipe details",         "recipe/"                     },
            { WowApiDataKey.spell_id,  "Spell details",          "spell/"                      },
        };
        #endregion Internal

        #region Ctor
        private WowApiDataType(string name, string path)
        {
            Name = name;
            Path = path;
        }
        #endregion Ctor

        #region Properties
        public string Name { get; private set; }
        public string Path { get; private set; }
        #endregion Properties

        #region Static Properties
        // global resources
        public static WowApiDataType CharacterRaces { get { return _types[WowApiDataKey.c_races]; } }
        public static WowApiDataType CharacterClasses { get { return _types[WowApiDataKey.c_class]; } }
        public static WowApiDataType CharacterAchievements { get { return _types[WowApiDataKey.c_achiev]; } }
        public static WowApiDataType ItemClasses { get { return _types[WowApiDataKey.i_class]; } }
        public static WowApiDataType Talents { get { return _types[WowApiDataKey.talents]; } }
        public static WowApiDataType PetTypes { get { return _types[WowApiDataKey.p_types]; } }
        // resources by id
        public static WowApiDataType Achievement { get { return _types[WowApiDataKey.achiev_id]; } }
        public static WowApiDataType Item { get { return _types[WowApiDataKey.item_id]; } }
        public static WowApiDataType Quest { get { return _types[WowApiDataKey.quest_id]; } }
        public static WowApiDataType Recipe { get { return _types[WowApiDataKey.recipe_id]; } }
        public static WowApiDataType Spell { get { return _types[WowApiDataKey.spell_id]; } }
        #endregion Static Properties
    }
}
