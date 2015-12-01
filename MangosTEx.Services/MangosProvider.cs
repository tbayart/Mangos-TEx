using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using Framework.Helpers;
using MangosData;
using MangosData.Context;
using MangosTEx.Services.Models;
using MySql.Data.MySqlClient;

namespace MangosTEx.Services
{
    public class MangosProvider : IDisposable
    {
        // localized tables
        // mangos.creature_ai_texts
        // mangos.db_script_string
        // mangos.game_event
        // mangos.locales_achievement_reward
        // mangos.locales_creature
        // mangos.locales_gameobject
        // mangos.locales_gossip_menu_option
        // mangos.locales_item
        // mangos.locales_npc_text
        // mangos.locales_page_text
        // mangos.locales_points_of_interest
        // mangos.locales_quest
        // mangos.mangos_string

        // *obsolete with rel21*
        // scriptdev2.custom_texts
        // scriptdev2.script_texts
        // scriptdev2.custom_texts

        #region Fields
        private MangosEntities _context;
        #endregion Fields

        #region Ctor
        public MangosProvider()
        {
            _context = GetContext();
        }
        #endregion Ctor

        #region Properties
        public static CultureInfo[] SupportedCultures { get { return LocalizationHelper.GetCultures(); } }
        public static CultureInfo DefaultCulture { get { return LocalizationHelper.DefaultCulture; } }
        private static Properties.Settings Settings { get { return Properties.Settings.Default; } }
        #endregion Properties

        #region Static Methods
        public static ConnectionStatus CheckDatabaseAccess()
        {
            try
            {
                using (var context = GetContext(3))
                {
                    var adapter = (System.Data.Entity.Infrastructure.IObjectContextAdapter)context;
                    if (adapter.ObjectContext.DatabaseExists() == false)
                        return new ConnectionStatus(false, "Failed to access database " + Settings.DatabaseName);
                }
            }
            catch (Exception ex)
            {
                return new ConnectionStatus(false, ex.Message);
            }
            return new ConnectionStatus(true, "Connection OK");
        }

        public static SecureString Decrypt(string data)
        {
            using (var algo = GetAlgorithm())
            {
                var result = algo.CreateDecryptor().Decrypt(data);
                return result;
            }
        }

        public static string Encrypt(SecureString data)
        {
            using (var algo = GetAlgorithm())
            {
                var result = algo.CreateEncryptor().Encrypt(data);
                return result;
            }
        }

        private static MangosEntities GetContext(uint timeout = 15)
        {
            // retrieve entity ConnectionString from configuration file
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["MangosEntities"];
            // parsing entity ConnectionString
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder(settings.ConnectionString);
            // using settings to create a ProviderConnectionString for MySql
            MySqlConnectionStringBuilder mysqlBuilder = new MySqlConnectionStringBuilder();
            mysqlBuilder.ConnectionTimeout = timeout;
            mysqlBuilder.Server = Settings.DatabaseHost;
            mysqlBuilder.Port = Settings.DatabasePort;
            mysqlBuilder.Database = Settings.DatabaseName;
            mysqlBuilder.UserID = Settings.DatabaseUsername;
            using (var algo = GetAlgorithm()) // decrypt database password into simple string
                mysqlBuilder.Password = algo.CreateDecryptor().DecryptClear(Settings.DatabasePassword);

            // setting up ProviderConnectionString in entity ConnectionString
            entityBuilder.ProviderConnectionString = mysqlBuilder.ConnectionString;
            // create the DbContext using ConnectionString
            return new MangosEntities(entityBuilder.ConnectionString);
        }

        // algorithm data for password encryption
        private static SymmetricAlgorithm GetAlgorithm()
        {
            var algo = Aes.Create();
            algo.Key = Convert.FromBase64String("6EvipoVj2gGtKUuIVHC5Tm3UIyBEmraD3UcJcrTArCc=");
            algo.IV = Convert.FromBase64String("RoDnj57f/XJS/7klvBEAPQ==");
            return algo;
        }
        #endregion Static Methods

        #region Items
        /// <summary>
        /// returns all database items localized for a given culture
        /// </summary>
        /// <param name="culture">the localization culture</param>
        /// <returns>all items in database</returns>
        public IEnumerable<Item> GetItems(CultureInfo culture)
        {
            return GetItems(_context.item_template.AsNoTracking(), culture);
        }

        /// <summary>
        /// returns items localized for a given culture
        /// </summary>
        /// <param name="source">the data source</param>
        /// <param name="culture">the localization culture</param>
        /// <returns>items converted from source</returns>
        private IEnumerable<Item> GetItems(IQueryable<item_template> source, CultureInfo culture)
        {
            var join = source
                .Join(_context.locales_item, o => o.entry, o => o.entry, (it, li) => new { it, li });

            switch (LocalizationHelper.GetOffset(culture))
            {
                case 1: return join.Select(o => new Item { Id = o.it.entry, Name = o.li.name_loc1, Description = o.li.description_loc1 });
                case 2: return join.Select(o => new Item { Id = o.it.entry, Name = o.li.name_loc2, Description = o.li.description_loc2 });
                case 3: return join.Select(o => new Item { Id = o.it.entry, Name = o.li.name_loc3, Description = o.li.description_loc3 });
                case 4: return join.Select(o => new Item { Id = o.it.entry, Name = o.li.name_loc4, Description = o.li.description_loc4 });
                case 5: return join.Select(o => new Item { Id = o.it.entry, Name = o.li.name_loc5, Description = o.li.description_loc5 });
                case 6: return join.Select(o => new Item { Id = o.it.entry, Name = o.li.name_loc6, Description = o.li.description_loc6 });
                case 7: return join.Select(o => new Item { Id = o.it.entry, Name = o.li.name_loc7, Description = o.li.description_loc7 });
                case 8: return join.Select(o => new Item { Id = o.it.entry, Name = o.li.name_loc8, Description = o.li.description_loc8 });
                default: return join.Select(o => new Item { Id = o.it.entry, Name = o.it.name, Description = o.it.description });
            }
        }

        /// <summary>
        /// update database with localized items information
        /// </summary>
        /// <param name="items">items with information to update</param>
        /// <param name="culture">culture for localization</param>
        /// <returns>updated items from database</returns>
        public IEnumerable<Item> UpdateItems(IEnumerable<Item> items, CultureInfo culture)
        {
            using (MangosEntities context = GetContext())
            {
                var item_templates = items.Join(context.item_template, o => o.Id, o => o.entry, (i, it) => new { it, i });
                var locales_items = items.Join(context.locales_item, o => o.Id, o => o.entry, (i, li) => new { li, i });

                switch (LocalizationHelper.GetOffset(culture))
                {
                    case 0: item_templates.ToList().ForEach(o => { o.it.name = o.i.Name; o.it.description = o.i.Description; }); break;
                    case 1: locales_items.ToList().ForEach(o => { o.li.name_loc1 = o.i.Name; o.li.description_loc1 = o.i.Description; }); break;
                    case 2: locales_items.ToList().ForEach(o => { o.li.name_loc2 = o.i.Name; o.li.description_loc2 = o.i.Description; }); break;
                    case 3: locales_items.ToList().ForEach(o => { o.li.name_loc3 = o.i.Name; o.li.description_loc3 = o.i.Description; }); break;
                    case 4: locales_items.ToList().ForEach(o => { o.li.name_loc4 = o.i.Name; o.li.description_loc4 = o.i.Description; }); break;
                    case 5: locales_items.ToList().ForEach(o => { o.li.name_loc5 = o.i.Name; o.li.description_loc5 = o.i.Description; }); break;
                    case 6: locales_items.ToList().ForEach(o => { o.li.name_loc6 = o.i.Name; o.li.description_loc6 = o.i.Description; }); break;
                    case 7: locales_items.ToList().ForEach(o => { o.li.name_loc7 = o.i.Name; o.li.description_loc7 = o.i.Description; }); break;
                    case 8: locales_items.ToList().ForEach(o => { o.li.name_loc8 = o.i.Name; o.li.description_loc8 = o.i.Description; }); break;
                    default: throw new NotImplementedException("Unsupported culture " + culture.Name);
                }

                context.SaveChanges();
            }

            List<int> ids = items.Select(o => o.Id).ToList();
            var updatedItems = _context.item_template.Where(o => ids.Contains(o.entry));
            return GetItems(updatedItems, culture);
        }
        #endregion Items

        #region GameObjects
        public IEnumerable<GameObject> GetGameObjects(CultureInfo culture)
        {
            var join = _context.gameobject_template.AsNoTracking()
                .Join(_context.locales_gameobject, o => o.entry, o => o.entry, (got, lgo) => new { got, lgo });

            switch (LocalizationHelper.GetOffset(culture))
            {
                case 1: return join.Select(o => new GameObject { Id = o.got.entry, Type = o.got.type, Name = o.lgo.name_loc1 });
                case 2: return join.Select(o => new GameObject { Id = o.got.entry, Type = o.got.type, Name = o.lgo.name_loc2 });
                case 3: return join.Select(o => new GameObject { Id = o.got.entry, Type = o.got.type, Name = o.lgo.name_loc3 });
                case 4: return join.Select(o => new GameObject { Id = o.got.entry, Type = o.got.type, Name = o.lgo.name_loc4 });
                case 5: return join.Select(o => new GameObject { Id = o.got.entry, Type = o.got.type, Name = o.lgo.name_loc5 });
                case 6: return join.Select(o => new GameObject { Id = o.got.entry, Type = o.got.type, Name = o.lgo.name_loc6 });
                case 7: return join.Select(o => new GameObject { Id = o.got.entry, Type = o.got.type, Name = o.lgo.name_loc7 });
                case 8: return join.Select(o => new GameObject { Id = o.got.entry, Type = o.got.type, Name = o.lgo.name_loc8 });
                default: return join.Select(o => new GameObject { Id = o.got.entry, Type = o.got.type, Name = o.got.name });
            }
        }

        public void UpdateGameObjectRelatedData(IEnumerable<GameObject> gameObject)
        {
        }
        #endregion GameObjects

        #region Quests
        /// <summary>
        /// returns all database quests localized for a given culture
        /// </summary>
        /// <param name="culture">the localization culture</param>
        /// <returns>all quests in database</returns>
        public IEnumerable<Quest> GetQuests(CultureInfo culture)
        {
            return GetQuests(_context.quest_template.AsNoTracking(), culture);
        }

        /// <summary>
        /// returns quests localized for a given culture
        /// </summary>
        /// <param name="source">the data source</param>
        /// <param name="culture">the localization culture</param>
        /// <returns>quests converted from source</returns>
        private IEnumerable<Quest> GetQuests(IQueryable<quest_template> source, CultureInfo culture)
        {
            var join = source
                .Join(_context.locales_quest, o => o.entry, o => o.entry, (it, li) => new { it, li });

            switch (LocalizationHelper.GetOffset(culture))
            {
                case 1: return join.Select(o => new Quest { Id = o.it.entry, Title = o.li.Title_loc1, Details = o.li.Details_loc1, Objectives = o.li.Objectives_loc1, OfferRewardText = o.li.OfferRewardText_loc1, RequestItemsText = o.li.RequestItemsText_loc1, EndText = o.li.EndText_loc1, CompletedText = o.li.CompletedText_loc1, ObjectiveText1 = o.li.ObjectiveText1_loc1, ObjectiveText2 = o.li.ObjectiveText2_loc1, ObjectiveText3 = o.li.ObjectiveText3_loc1, ObjectiveText4 = o.li.ObjectiveText4_loc1 });
                case 2: return join.Select(o => new Quest { Id = o.it.entry, Title = o.li.Title_loc2, Details = o.li.Details_loc2, Objectives = o.li.Objectives_loc2, OfferRewardText = o.li.OfferRewardText_loc2, RequestItemsText = o.li.RequestItemsText_loc2, EndText = o.li.EndText_loc2, CompletedText = o.li.CompletedText_loc2, ObjectiveText1 = o.li.ObjectiveText1_loc2, ObjectiveText2 = o.li.ObjectiveText2_loc2, ObjectiveText3 = o.li.ObjectiveText3_loc2, ObjectiveText4 = o.li.ObjectiveText4_loc2 });
                case 3: return join.Select(o => new Quest { Id = o.it.entry, Title = o.li.Title_loc3, Details = o.li.Details_loc3, Objectives = o.li.Objectives_loc3, OfferRewardText = o.li.OfferRewardText_loc3, RequestItemsText = o.li.RequestItemsText_loc3, EndText = o.li.EndText_loc3, CompletedText = o.li.CompletedText_loc3, ObjectiveText1 = o.li.ObjectiveText1_loc3, ObjectiveText2 = o.li.ObjectiveText2_loc3, ObjectiveText3 = o.li.ObjectiveText3_loc3, ObjectiveText4 = o.li.ObjectiveText4_loc3 });
                case 4: return join.Select(o => new Quest { Id = o.it.entry, Title = o.li.Title_loc4, Details = o.li.Details_loc4, Objectives = o.li.Objectives_loc4, OfferRewardText = o.li.OfferRewardText_loc4, RequestItemsText = o.li.RequestItemsText_loc4, EndText = o.li.EndText_loc4, CompletedText = o.li.CompletedText_loc4, ObjectiveText1 = o.li.ObjectiveText1_loc4, ObjectiveText2 = o.li.ObjectiveText2_loc4, ObjectiveText3 = o.li.ObjectiveText3_loc4, ObjectiveText4 = o.li.ObjectiveText4_loc4 });
                case 5: return join.Select(o => new Quest { Id = o.it.entry, Title = o.li.Title_loc5, Details = o.li.Details_loc5, Objectives = o.li.Objectives_loc5, OfferRewardText = o.li.OfferRewardText_loc5, RequestItemsText = o.li.RequestItemsText_loc5, EndText = o.li.EndText_loc5, CompletedText = o.li.CompletedText_loc5, ObjectiveText1 = o.li.ObjectiveText1_loc5, ObjectiveText2 = o.li.ObjectiveText2_loc5, ObjectiveText3 = o.li.ObjectiveText3_loc5, ObjectiveText4 = o.li.ObjectiveText4_loc5 });
                case 6: return join.Select(o => new Quest { Id = o.it.entry, Title = o.li.Title_loc6, Details = o.li.Details_loc6, Objectives = o.li.Objectives_loc6, OfferRewardText = o.li.OfferRewardText_loc6, RequestItemsText = o.li.RequestItemsText_loc6, EndText = o.li.EndText_loc6, CompletedText = o.li.CompletedText_loc6, ObjectiveText1 = o.li.ObjectiveText1_loc6, ObjectiveText2 = o.li.ObjectiveText2_loc6, ObjectiveText3 = o.li.ObjectiveText3_loc6, ObjectiveText4 = o.li.ObjectiveText4_loc6 });
                case 7: return join.Select(o => new Quest { Id = o.it.entry, Title = o.li.Title_loc7, Details = o.li.Details_loc7, Objectives = o.li.Objectives_loc7, OfferRewardText = o.li.OfferRewardText_loc7, RequestItemsText = o.li.RequestItemsText_loc7, EndText = o.li.EndText_loc7, CompletedText = o.li.CompletedText_loc7, ObjectiveText1 = o.li.ObjectiveText1_loc7, ObjectiveText2 = o.li.ObjectiveText2_loc7, ObjectiveText3 = o.li.ObjectiveText3_loc7, ObjectiveText4 = o.li.ObjectiveText4_loc7 });
                case 8: return join.Select(o => new Quest { Id = o.it.entry, Title = o.li.Title_loc8, Details = o.li.Details_loc8, Objectives = o.li.Objectives_loc8, OfferRewardText = o.li.OfferRewardText_loc8, RequestItemsText = o.li.RequestItemsText_loc8, EndText = o.li.EndText_loc8, CompletedText = o.li.CompletedText_loc8, ObjectiveText1 = o.li.ObjectiveText1_loc8, ObjectiveText2 = o.li.ObjectiveText2_loc8, ObjectiveText3 = o.li.ObjectiveText3_loc8, ObjectiveText4 = o.li.ObjectiveText4_loc8 });
                default: return join.Select(o => new Quest { Id = o.it.entry, Title = o.it.Title, Details = o.it.Details, Objectives = o.it.Objectives, OfferRewardText = o.it.OfferRewardText, RequestItemsText = o.it.RequestItemsText, EndText = o.it.EndText, CompletedText = o.it.CompletedText, ObjectiveText1 = o.it.ObjectiveText1, ObjectiveText2 = o.it.ObjectiveText2, ObjectiveText3 = o.it.ObjectiveText3, ObjectiveText4 = o.it.ObjectiveText4 });
            }
        }

        /// <summary>
        /// update database with localized quests information
        /// </summary>
        /// <param name="quests">quests with information to update</param>
        /// <param name="culture">culture for localization</param>
        /// <returns>updated quests from database</returns>
        public IEnumerable<Quest> UpdateQuests(IEnumerable<Quest> quests, CultureInfo culture)
        {
            using (MangosEntities context = GetContext())
            {
                var quest_templates = quests.Join(context.quest_template, o => o.Id, o => o.entry, (i, it) => new { it, i });
                var locales_quests = quests.Join(context.locales_quest, o => o.Id, o => o.entry, (i, li) => new { li, i });

                switch (LocalizationHelper.GetOffset(culture))
                {
                    case 0: quest_templates.ToList().ForEach(o => { o.it.Title = o.i.Title; o.it.Details = o.i.Details; o.it.Objectives = o.i.Objectives; o.it.OfferRewardText = o.i.OfferRewardText; o.it.RequestItemsText = o.i.RequestItemsText; o.it.EndText = o.i.EndText; o.it.CompletedText = o.i.CompletedText; o.it.ObjectiveText1 = o.i.ObjectiveText1; o.it.ObjectiveText2 = o.i.ObjectiveText2; o.it.ObjectiveText3 = o.i.ObjectiveText3; o.it.ObjectiveText4 = o.i.ObjectiveText4; }); break;
                    case 1: locales_quests.ToList().ForEach(o => { o.li.Title_loc1 = o.i.Title; o.li.Details_loc1 = o.i.Details; o.li.Objectives_loc1 = o.i.Objectives; o.li.OfferRewardText_loc1 = o.i.OfferRewardText; o.li.RequestItemsText_loc1 = o.i.RequestItemsText; o.li.EndText_loc1 = o.i.EndText; o.li.CompletedText_loc1 = o.i.CompletedText; o.li.ObjectiveText1_loc1 = o.i.ObjectiveText1; o.li.ObjectiveText2_loc1 = o.i.ObjectiveText2; o.li.ObjectiveText3_loc1 = o.i.ObjectiveText3; o.li.ObjectiveText4_loc1 = o.i.ObjectiveText4; }); break;
                    case 2: locales_quests.ToList().ForEach(o => { o.li.Title_loc2 = o.i.Title; o.li.Details_loc2 = o.i.Details; o.li.Objectives_loc2 = o.i.Objectives; o.li.OfferRewardText_loc2 = o.i.OfferRewardText; o.li.RequestItemsText_loc2 = o.i.RequestItemsText; o.li.EndText_loc2 = o.i.EndText; o.li.CompletedText_loc2 = o.i.CompletedText; o.li.ObjectiveText1_loc2 = o.i.ObjectiveText1; o.li.ObjectiveText2_loc2 = o.i.ObjectiveText2; o.li.ObjectiveText3_loc2 = o.i.ObjectiveText3; o.li.ObjectiveText4_loc2 = o.i.ObjectiveText4; }); break;
                    case 3: locales_quests.ToList().ForEach(o => { o.li.Title_loc3 = o.i.Title; o.li.Details_loc3 = o.i.Details; o.li.Objectives_loc3 = o.i.Objectives; o.li.OfferRewardText_loc3 = o.i.OfferRewardText; o.li.RequestItemsText_loc3 = o.i.RequestItemsText; o.li.EndText_loc3 = o.i.EndText; o.li.CompletedText_loc3 = o.i.CompletedText; o.li.ObjectiveText1_loc3 = o.i.ObjectiveText1; o.li.ObjectiveText2_loc3 = o.i.ObjectiveText2; o.li.ObjectiveText3_loc3 = o.i.ObjectiveText3; o.li.ObjectiveText4_loc3 = o.i.ObjectiveText4; }); break;
                    case 4: locales_quests.ToList().ForEach(o => { o.li.Title_loc4 = o.i.Title; o.li.Details_loc4 = o.i.Details; o.li.Objectives_loc4 = o.i.Objectives; o.li.OfferRewardText_loc4 = o.i.OfferRewardText; o.li.RequestItemsText_loc4 = o.i.RequestItemsText; o.li.EndText_loc4 = o.i.EndText; o.li.CompletedText_loc4 = o.i.CompletedText; o.li.ObjectiveText1_loc4 = o.i.ObjectiveText1; o.li.ObjectiveText2_loc4 = o.i.ObjectiveText2; o.li.ObjectiveText3_loc4 = o.i.ObjectiveText3; o.li.ObjectiveText4_loc4 = o.i.ObjectiveText4; }); break;
                    case 5: locales_quests.ToList().ForEach(o => { o.li.Title_loc5 = o.i.Title; o.li.Details_loc5 = o.i.Details; o.li.Objectives_loc5 = o.i.Objectives; o.li.OfferRewardText_loc5 = o.i.OfferRewardText; o.li.RequestItemsText_loc5 = o.i.RequestItemsText; o.li.EndText_loc5 = o.i.EndText; o.li.CompletedText_loc5 = o.i.CompletedText; o.li.ObjectiveText1_loc5 = o.i.ObjectiveText1; o.li.ObjectiveText2_loc5 = o.i.ObjectiveText2; o.li.ObjectiveText3_loc5 = o.i.ObjectiveText3; o.li.ObjectiveText4_loc5 = o.i.ObjectiveText4; }); break;
                    case 6: locales_quests.ToList().ForEach(o => { o.li.Title_loc6 = o.i.Title; o.li.Details_loc6 = o.i.Details; o.li.Objectives_loc6 = o.i.Objectives; o.li.OfferRewardText_loc6 = o.i.OfferRewardText; o.li.RequestItemsText_loc6 = o.i.RequestItemsText; o.li.EndText_loc6 = o.i.EndText; o.li.CompletedText_loc6 = o.i.CompletedText; o.li.ObjectiveText1_loc6 = o.i.ObjectiveText1; o.li.ObjectiveText2_loc6 = o.i.ObjectiveText2; o.li.ObjectiveText3_loc6 = o.i.ObjectiveText3; o.li.ObjectiveText4_loc6 = o.i.ObjectiveText4; }); break;
                    case 7: locales_quests.ToList().ForEach(o => { o.li.Title_loc7 = o.i.Title; o.li.Details_loc7 = o.i.Details; o.li.Objectives_loc7 = o.i.Objectives; o.li.OfferRewardText_loc7 = o.i.OfferRewardText; o.li.RequestItemsText_loc7 = o.i.RequestItemsText; o.li.EndText_loc7 = o.i.EndText; o.li.CompletedText_loc7 = o.i.CompletedText; o.li.ObjectiveText1_loc7 = o.i.ObjectiveText1; o.li.ObjectiveText2_loc7 = o.i.ObjectiveText2; o.li.ObjectiveText3_loc7 = o.i.ObjectiveText3; o.li.ObjectiveText4_loc7 = o.i.ObjectiveText4; }); break;
                    case 8: locales_quests.ToList().ForEach(o => { o.li.Title_loc8 = o.i.Title; o.li.Details_loc8 = o.i.Details; o.li.Objectives_loc8 = o.i.Objectives; o.li.OfferRewardText_loc8 = o.i.OfferRewardText; o.li.RequestItemsText_loc8 = o.i.RequestItemsText; o.li.EndText_loc8 = o.i.EndText; o.li.CompletedText_loc8 = o.i.CompletedText; o.li.ObjectiveText1_loc8 = o.i.ObjectiveText1; o.li.ObjectiveText2_loc8 = o.i.ObjectiveText2; o.li.ObjectiveText3_loc8 = o.i.ObjectiveText3; o.li.ObjectiveText4_loc8 = o.i.ObjectiveText4; }); break;
                    default: throw new NotImplementedException("Unsupported culture " + culture.Name);
                }

                context.SaveChanges();
            }

            List<int> ids = quests.Select(o => o.Id).ToList();
            var updatedQuests = _context.quest_template.Where(o => ids.Contains(o.entry));
            return GetQuests(updatedQuests, culture);
        }
        #endregion Quests

        #region Achievements
        public IEnumerable<Achievement> GetAchievements(CultureInfo culture)
        {
            return _context.achievement_reward.Select(o => new Achievement { Id = o.entry, Subject = o.subject, Text = o.text });
        }
        #endregion Achievements

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose the DbContext
                _context.Dispose();
                _context = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable
    }
}
