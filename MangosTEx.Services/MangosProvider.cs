using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.EntityClient;
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
            using(MangosEntities context = GetContext())
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
