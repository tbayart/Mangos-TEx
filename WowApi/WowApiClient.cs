using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Framework.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WowApi.Models;
using WowFramework.Helpers;

namespace WowApi
{
    public class WowApiClient
    {
        string _urlPrefix;

        public WowApiClient()
            : this(LocaleHelpers.DefaultCulture)
        {
        }

        public WowApiClient(CultureInfo locale)
        {
            SetLocale(locale);
        }

        public string SetLocale(CultureInfo culture)
        {
            string host = LocaleHelpers.GetHost(culture);
            _urlPrefix = string.Format("https://{0}/api/wow/", host);
            return host;
        }

        public string GetDataById(WowApiDataType type, int? id)
        {
            if (type == null)
                throw new ArgumentNullException("type", "GetDataById : null type");

            string address = string.Concat(_urlPrefix, type.Path);

            if (address.EndsWith("/") == true)
            {
                if(id.HasValue == false)
                    throw new ArgumentNullException("id", "GetDataById : id needed with " + type.Name);

                address += id.Value;
            }

            return WebRequestHelpers.DownloadString(address);
        }

        public Character CharTest(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            string data = sr.ReadToEnd();
            return JsonConvert.DeserializeObject<Character>(data);
        }

        // The Character Profile API is the primary way to access character information.
        // http://eu.battle.net/api/wow/character/Blackmoore/Nerd?fields=achievements,appearance,feed,guild,hunterPets,items,mounts,pets,petSlots,professions,progression,pvp,quests,reputation,stats,talents,titles
        public Character GetCharacterData(string realm, string name, WowApiCharacterDataField[] fields = null)
        {
            string fieldsString = fields == null
                ? string.Empty
                : string.Concat("?fields=", string.Join(",", fields.Select(o => o.Field)));

            string address = string.Format("{0}character/{1}/{2}{3}", _urlPrefix, realm.Replace(" ", ""), name, fieldsString);
            string data = WebRequestHelpers.DownloadString(address);

            return JsonConvert.DeserializeObject<Character>(data);
        }

        // This provides the data about an individual pet species.
        // http://eu.battle.net/api/wow/battlePet/stats/258?level=25&breedId=5&qualityId=4
        // level : The Pet's level (default 1)
        // breedId : The Pet's breed (can be retrieved from the character profile api) (default 3)
        // qualityId : The Pet's quality (can be retrieved from the character profile api) (default 1)
        public BattlePet GetBattlePetData(int id, int level = 1, int breed = 3, int quality = 1)
        {
            string address = string.Format("{0}battlePet/stats/{1}?level={2}&breedId={3}&qualityId={4}", _urlPrefix, id, level, breed, quality);
            string data = WebRequestHelpers.DownloadString(address);
            return JsonConvert.DeserializeObject<BattlePet>(data);
        }

        // This provides data about an individual achievement.
        // http://eu.battle.net/api/wow/achievement/2144
        // The item API provides data about items and item sets.
        // http://eu.battle.net/api/wow/item/18803
        public Item GetItem(int id)
        {
            string address = string.Format("{0}item/{1}", _urlPrefix, id);
            string data = WebRequestHelpers.DownloadString(address);
            return JsonConvert.DeserializeObject<Item>(data);
        }

        // The quest API provides detailed quest information.
        // http://eu.battle.net/api/wow/quest/13146
        // The recipe API provides basic recipe information.
        // http://eu.battle.net/api/wow/recipe/33994
        // The spell API provides some information about spells.
        // http://eu.battle.net/api/wow/spell/8056

        // Data Resources
        // The character races data API provides a list of character races.
        // http://eu.battle.net/api/wow/data/character/races
        // The character classes data API provides a list of character classes.
        // http://eu.battle.net/api/wow/data/character/classes
        // The character achievements data API provides a list of all of the achievements that characters can earn as well as the category structure and hierarchy.
        // http://eu.battle.net/api/wow/data/character/achievements
        // The item classes data API provides a list of item classes.
        // http://eu.battle.net/api/wow/data/item/classes
        // The talents data API provides a list of talents, specs and glyphs for each class.
        // http://eu.battle.net/api/wow/data/talents
        // The different bat pet types (including what they are strong and weak against)
        // http://eu.battle.net/api/wow/data/pet/types
    }
}
