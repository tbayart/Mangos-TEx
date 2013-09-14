using System.Collections.Generic;
using System.Linq;

namespace WowApi
{
    public class WowApiCharacterDataField
    {
        #region Internal
        internal enum WowApiCharacterDataKey
        {
            achievements,
            appearance,
            feed,
            guild,
            hunterPets,
            items,
            mounts,
            pets,
            petSlots,
            professions,
            progression,
            pvp,
            quests,
            reputation,
            stats,
            talents,
            titles
        }

        internal class WowApiCharacterDataFields : Dictionary<WowApiCharacterDataKey, WowApiCharacterDataField>
        {
            public void Add(WowApiCharacterDataKey key, string name, string field) { Add(key, new WowApiCharacterDataField(name, field)); }
        }

        private static WowApiCharacterDataFields _fields = new WowApiCharacterDataFields
        {
            { WowApiCharacterDataKey.achievements, "Achievements", "achievements" },
            { WowApiCharacterDataKey.appearance,   "Appearance",   "appearance"   },
            { WowApiCharacterDataKey.feed,         "Feed",         "feed"         },
            { WowApiCharacterDataKey.guild,        "Guild",        "guild"        },
            { WowApiCharacterDataKey.hunterPets,   "Hunter Pets",  "hunterPets"   },
            { WowApiCharacterDataKey.items,        "Items",        "items"        },
            { WowApiCharacterDataKey.mounts,       "Mounts",       "mounts"       },
            { WowApiCharacterDataKey.pets,         "Pets",         "pets"         },
            { WowApiCharacterDataKey.petSlots,     "Pet Slots",    "petSlots"     },
            { WowApiCharacterDataKey.professions,  "Professions",  "professions"  },
            { WowApiCharacterDataKey.progression,  "Progression",  "progression"  },
            { WowApiCharacterDataKey.pvp,          "Pvp",          "pvp"          },
            { WowApiCharacterDataKey.quests,       "Quests",       "quests"       },
            { WowApiCharacterDataKey.reputation,   "Reputation",   "reputation"   },
            { WowApiCharacterDataKey.stats,        "Stats",        "stats"        },
            { WowApiCharacterDataKey.talents,      "Talents",      "talents"      },
            { WowApiCharacterDataKey.titles,       "Titles",       "titles"       },
        };
        #endregion Internal

        #region Ctor
        private WowApiCharacterDataField(string name, string field)
        {
            Name = name;
            Field = field;
        }
        #endregion Ctor

        #region Properties
        public string Name { get; private set; }
        public string Field { get; private set; }
        #endregion Properties

        #region Static Properties
        public static WowApiCharacterDataField Achievements { get { return _fields[WowApiCharacterDataKey.achievements]; } }
        public static WowApiCharacterDataField Appearance { get { return _fields[WowApiCharacterDataKey.appearance]; } }
        public static WowApiCharacterDataField Feed { get { return _fields[WowApiCharacterDataKey.feed]; } }
        public static WowApiCharacterDataField Guild { get { return _fields[WowApiCharacterDataKey.guild]; } }
        public static WowApiCharacterDataField HunterPets { get { return _fields[WowApiCharacterDataKey.hunterPets]; } }
        public static WowApiCharacterDataField Items { get { return _fields[WowApiCharacterDataKey.items]; } }
        public static WowApiCharacterDataField Mounts { get { return _fields[WowApiCharacterDataKey.mounts]; } }
        public static WowApiCharacterDataField Pets { get { return _fields[WowApiCharacterDataKey.pets]; } }
        public static WowApiCharacterDataField PetSlots { get { return _fields[WowApiCharacterDataKey.petSlots]; } }
        public static WowApiCharacterDataField Professions { get { return _fields[WowApiCharacterDataKey.professions]; } }
        public static WowApiCharacterDataField Progression { get { return _fields[WowApiCharacterDataKey.progression]; } }
        public static WowApiCharacterDataField Pvp { get { return _fields[WowApiCharacterDataKey.pvp]; } }
        public static WowApiCharacterDataField Quests { get { return _fields[WowApiCharacterDataKey.quests]; } }
        public static WowApiCharacterDataField Reputation { get { return _fields[WowApiCharacterDataKey.reputation]; } }
        public static WowApiCharacterDataField Stats { get { return _fields[WowApiCharacterDataKey.stats]; } }
        public static WowApiCharacterDataField Talents { get { return _fields[WowApiCharacterDataKey.talents]; } }
        public static WowApiCharacterDataField Titles { get { return _fields[WowApiCharacterDataKey.titles]; } }

        /// <summary>
        /// Chose this to include all fields
        /// </summary>
        public static WowApiCharacterDataField[] All { get { return _fields.Values.ToArray(); } }
        #endregion Static Properties
    }
}
