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
        private static Func<item_template, Item> _getItem = o => new Item { Id = o.entry, Name = o.name, Description = o.description };
        private static Func<locales_item, Item>[] _getLoc =
        {
            o => new Item { Id = o.entry, Name = string.Empty, Description = string.Empty },
            o => new Item { Id = o.entry, Name = o.name_loc1, Description = o.description_loc1 },
            o => new Item { Id = o.entry, Name = o.name_loc2, Description = o.description_loc2 },
            o => new Item { Id = o.entry, Name = o.name_loc3, Description = o.description_loc3 },
            o => new Item { Id = o.entry, Name = o.name_loc4, Description = o.description_loc4 },
            o => new Item { Id = o.entry, Name = o.name_loc5, Description = o.description_loc5 },
            o => new Item { Id = o.entry, Name = o.name_loc6, Description = o.description_loc6 },
            o => new Item { Id = o.entry, Name = o.name_loc7, Description = o.description_loc7 },
            o => new Item { Id = o.entry, Name = o.name_loc8, Description = o.description_loc8 },
        };

        public IEnumerable<Item> GetItems(CultureInfo culture)
        {
            int offset = LocalizationHelper.GetOffset(culture);
            if (offset == 0)
            {
                return _context.item_template
                    .AsNoTracking()
                    .Select(_getItem);
            }

            Func<locales_item, Item> selector = _getLoc[offset];
            var items = _context.locales_item
                .AsNoTracking()
                .Select(selector);

            return items;
        }
        #endregion Items

        #region GameObjects
        public IEnumerable<GameObject> GetGameObjects()
        {
            var gameobjects = _context.gameobject_template
                .AsNoTracking()
                .Select(o => new GameObject { Id = o.entry, Name = o.name, Type = o.type });

            return gameobjects;
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
