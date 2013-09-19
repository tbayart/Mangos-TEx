using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Framework.Helpers;
using Framework.MVVM;
using Newtonsoft.Json;
using WowApi.JsonConverters;
using WowApi.Models;
using WowFramework.Helpers;

namespace WowApi
{
    /// <summary>
    /// Client class to make a request to WoW API
    /// </summary>
    public class WowApiClient : NotificationObject
    {
        #region Fields
        /// <summary>
        /// Stores url prefix to make a request
        /// </summary>
        private string _urlPrefix;
        private string _urlLocaleSuffix;
        #endregion Fields

        #region Ctor
        /// <summary>
        /// This constructor initialize the client with default culture
        /// </summary>
        public WowApiClient()
            : this(LocaleHelpers.DefaultCulture)
        {
        }

        /// <summary>
        /// This constructor initialize the client with specified culture
        /// If the culture is not supported, the client switch to default one
        /// If the use of the locale suffix is disabled, WoW API will leave localized data untranslated
        /// </summary>
        /// <param name="locale">The requested culture</param>
        /// <param name="useLocaleSuffix">Specify if the locale suffix must be added to the request url</param>
        public WowApiClient(CultureInfo locale, bool useLocaleSuffix = true)
        {
            _useLocaleSuffix = useLocaleSuffix;
            CurrentLocale = locale;
        }
        #endregion Ctor

        #region Properties
        /// <summary>
        /// The current culture
        /// </summary>
        public CultureInfo CurrentLocale
        {
            get { return _currentLocale; }
            set
            {
                value = LocaleHelpers.ValidateCulture(value);
                _currentLocale = value;
                RaisePropertyChanged(() => CurrentLocale);
                CurrentLocaleChanged();
            }
        }
        private CultureInfo _currentLocale;

        /// <summary>
        /// Specify if the locale suffix must be added to the request url
        /// </summary>
        public bool UseLocaleSuffix
        {
            get { return _useLocaleSuffix; }
            set
            {
                _useLocaleSuffix = value;
                RaisePropertyChanged(() => UseLocaleSuffix);
            }
        }
        private bool _useLocaleSuffix;
        #endregion Properties

        #region Private Methods
        /// <summary>
        /// Updates _urlPrefix on CurrentLocale change
        /// </summary>
        private void CurrentLocaleChanged()
        {
            _urlPrefix = LocaleHelpers.GetHost(CurrentLocale);
            _urlLocaleSuffix = string.Concat("locale=", LocaleHelpers.GetLocale(CurrentLocale));
        }

        /// <summary>
        /// Craft the url to request data from WoW API
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="args">Arguments for data type</param>
        /// <returns>The formated url</returns>
        private string GetUrl<T>(params object[] args)
        {
            // retrieve the WowApiDataType to craft the url
            var type = WowApiDataType.AllDataTypes.FirstOrDefault(o => o.Type == typeof(T));
            if (type == null)
                throw new NotImplementedException("GetUrl : Unsupported data type " + typeof(T).FullName);

            // format the url using path format and args
            string url = string.Concat(_urlPrefix, string.Format(type.Path, args));
            // check for locale suffix
            if (UseLocaleSuffix == true)
            {
                // check for parameters to choose a locale token
                string localeToken = url.Contains('?') ? "&" : "?";
                // complete the url
                url = string.Concat(url, localeToken, _urlLocaleSuffix);
            }
            // return the crafted url
            return url;
        }

        /// <summary>
        /// Download data from its type
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="args">Arguments for data type</param>
        /// <returns>Downloaded data</returns>
        private T GetData<T>(params object[] args)
            where T : class
        {
            string url = GetUrl<T>(args);
            string data = WebRequestHelpers.DownloadString(url);
            return JsonConvert.DeserializeObject<T>(data);
        }

        /// <summary>
        /// Download data list from its parameter type
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="converter">JsonConverter for data type</param>
        /// <returns>Downloaded data list</returns>
        private List<T> GetData<T>(JsonConverter converter)
        {
            string url = GetUrl<T>();
            string data = WebRequestHelpers.DownloadString(url);
            return JsonConvert.DeserializeObject<List<T>>(data, new[] { converter });
        }

        /// <summary>
        /// Download data list from its parameter type and a container
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="containerName">The container name in json structure</param>
        /// <returns>Downloaded data list</returns>
        private List<T> GetData<T>(string containerName)
        {
            return GetData<T>(new ContainerConverter<T>(containerName));
        }
        #endregion Private Methods

        #region Public Methods
        /// <summary>
        /// This provides data about an individual achievement
        /// ie : http://eu.battle.net/api/wow/achievement/2144
        /// </summary>
        /// <param name="id">The achievement id</param>
        /// <returns>The individual achievement</returns>
        public Achievement GetAchievement(int id)
        {
            return GetData<Achievement>(id);
        }

        /// <summary>
        /// This provides data about a individual battle pet ability ID.
        /// http://eu.battle.net/api/wow/battlePet/ability/640
        /// </summary>
        /// <param name="id">The ability id</param>
        /// <returns>The battle pet ability</returns>
        public BattlePetAbility GetBattlePetAbility(int id)
        {
            return GetData<BattlePetAbility>(id);
        }

        /// <summary>
        /// This provides the data about an individual pet species.
        /// The species ids can be found your character profile using the options pets field.
        /// Each species also has data about what it's 6 abilities are.
        /// http://eu.battle.net/api/wow/battlePet/species/258
        /// </summary>
        /// <param name="id">The individual pet species id</param>
        /// <returns>The individual pet species</returns>
        public BattlePetSpecies GetBattlePetSpecies(int id)
        {
            return GetData<BattlePetSpecies>(id);
        }

        /// <summary>
        /// This provides the stats about an individual pet species.
        /// http://eu.battle.net/api/wow/battlePet/stats/258?level=25&breedId=5&qualityId=4
        /// </summary>
        /// <param name="id">The individual pet species id</param>
        /// <param name="level">The Pet's level (default 1)</param>
        /// <param name="breed">The Pet's breed (can be retrieved from the character profile api) (default 3)</param>
        /// <param name="quality">The Pet's quality (can be retrieved from the character profile api) (default 1)</param>
        /// <returns>The individual pet species stats</returns>
        public BattlePetStats GetBattlePetStats(int id, int level = 1, int breed = 3, int quality = 1)
        {
            return GetData<BattlePetStats>(id, level, breed, quality);
        }

        /// <summary>
        // The Character Profile API is the primary way to access character information.
        // http://eu.battle.net/api/wow/character/Medivh/Uther?fields=achievements,appearance,feed,guild,hunterPets,items,mounts,pets,petSlots,professions,progression,pvp,quests,reputation,stats,talents,titles
        /// </summary>
        /// <param name="realm">The character realm</param>
        /// <param name="name">The character name</param>
        /// <param name="fields">Optional data fields</param>
        /// <returns>The downloaded character data</returns>
        public Character GetCharacter(string realm, string name, WowApiCharacterDataField[] fields = null)
        {
            // remove spaces from realm name
            realm = realm.Replace(" ", "");
            // concatenates the optional fields
            string fieldsString = fields == null
                ? string.Empty
                : string.Concat("?fields=", string.Join(",", fields.Select(o => o.Field)));
            // request data
            return GetData<Character>(realm, name, fieldsString);
        }

        /// <summary>
        /// Thitem API provides data about items
        /// http://eu.battle.net/api/wow/item/18803
        /// </summary>
        /// <param name="id">The item id</param>
        /// <returns>The item</returns>
        public Item GetItem(int id)
        {
            return GetData<Item>(id);
        }

        /// <summary>
        /// This provides the data for an item set.
        /// http://eu.battle.net/api/wow/item/set/1060
        /// </summary>
        /// <param name="id">The item set id</param>
        /// <returns>The item set</returns>
        public ItemSet GetItemSet(int id)
        {
            return GetData<ItemSet>(id);
        }

        /// <summary>
        /// The quest API provides detailed quest information.
        /// http://eu.battle.net/api/wow/quest/13146
        /// </summary>
        /// <param name="id">The quest id</param>
        /// <returns>The quest</returns>
        public Quest GetQuest(int id)
        {
            return GetData<Quest>(id);
        }

        /// <summary>
        /// The recipe API provides basic recipe information.
        /// http://eu.battle.net/api/wow/recipe/33994
        /// </summary>
        /// <param name="id">The recipe id</param>
        /// <returns>The recipe</returns>
        public Recipe GetRecipe(int id)
        {
            return GetData<Recipe>(id);
        }

        /// <summary>
        /// The spell API provides some information about spells.
        /// http://eu.battle.net/api/wow/spell/8056
        /// </summary>
        /// <param name="id">The spell id</param>
        /// <returns>The spell</returns>
        public Spell GetSpell(int id)
        {
            return GetData<Spell>(id);
        }

        /// <summary>
        /// The character races data API provides a list of character races.
        /// http://eu.battle.net/api/wow/data/character/races
        /// </summary>
        /// <returns>A list of all character races</returns>
        public List<CharacterRace> GetRaces()
        {
            return GetData<CharacterRace>("races");
        }

        /// <summary>
        /// The character classes data API provides a list of character classes.
        /// http://eu.battle.net/api/wow/data/character/classes
        /// </summary>
        /// <returns>A list of all character classes</returns>
        public List<CharacterClass> GetClasses()
        {
            return GetData<CharacterClass>("classes");
        }

        /// <summary>
        /// The character achievements data API provides a list of all of the achievements
        /// that characters can earn as well as the category structure and hierarchy.
        /// http://eu.battle.net/api/wow/data/character/achievements
        /// </summary>
        /// <returns>A list of all achievements and categories</returns>
        public List<AchievementNode> GetAchievements()
        {
            return GetData<AchievementNode>(new AchievementsConverter());
        }

        /// <summary>
        /// The item classes data API provides a list of item classes.
        /// http://eu.battle.net/api/wow/data/item/classes
        /// </summary>
        /// <returns>A list of all item classes</returns>
        public List<ItemClass> GetItemClasses()
        {
            return GetData<ItemClass>("classes");
        }

        /// <summary>
        /// The talents data API provides a list of talents, specs and glyphs for each class.
        /// http://eu.battle.net/api/wow/data/talents
        /// </summary>
        /// <returns>A list of all talents, specs and glyphs</returns>
        public List<Talent> GetTalents()
        {
            return GetData<Talent>(new TalentsConverter());
        }

        /// <summary>
        /// The different bat pet types (including what they are strong and weak against)
        /// http://eu.battle.net/api/wow/data/pet/types
        /// </summary>
        /// <returns>A list of all pet types</returns>
        public List<PetType> GetPetTypes()
        {
            return GetData<PetType>("petTypes");
        }
        #endregion Public Methods
    }
}
