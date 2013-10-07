using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.EntityClient;
using System.Globalization;
using System.Linq;
using MangosData.Context;
using MangosTEx.Services.ModelBase;
using MangosTEx.Services.Models;
using MySql.Data.MySqlClient;

namespace MangosTEx.Services
{
    public class MangosProvider
    {
        #region Ctor
        public MangosProvider()
        {
        }
        #endregion Ctor

        #region Properties
        private static Properties.Settings Settings { get { return Properties.Settings.Default; } }
        #endregion Properties

        #region Helpers
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
            mysqlBuilder.Password = Settings.DatabasePassword;
            // setting up ProviderConnectionString in entity ConnectionString
            entityBuilder.ProviderConnectionString = mysqlBuilder.ConnectionString;
            // create the DbContext using ConnectionString
            return new MangosEntities(entityBuilder.ConnectionString);
        }

        public static string CheckDatabaseAccess()
        {
            try
            {
                using (var context = GetContext(1))
                {
                    var adapter = (System.Data.Entity.Infrastructure.IObjectContextAdapter)context;
                    if (adapter.ObjectContext.DatabaseExists() == true)
                        return string.Empty;
                    return "Failed to access database " + Settings.DatabaseName;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion Helpers

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

        public IEnumerable<Item> GetItems()
        {
            var items = GetContext().item_template
                .AsNoTracking()
                .Select(_getItem);

            return items;
        }

        public void ItemsLocale(IEnumerable<Item> items, CultureInfo culture)
        {
            int offset = WowFramework.Helpers.LocaleHelpers.GetOffset(culture);
            if (offset >= _getLoc.Length)
                offset = 0;

            Func<locales_item, Item> selector = _getLoc[offset];
            var entries = items.Select(o => o.Id).ToList();
            var locales = GetContext().locales_item
                .AsNoTracking()
                //.Where(o => entries.Contains(o.entry))
                .Select(selector)
                .ToList()
                .AsParallel();

            items.AsParallel()
                .Join(locales, o => o.Id, o => o.Id, (item, loc) =>
                    {
                        item.LocalizedName = loc.Name;
                        item.LocalizedDescription = loc.Description;
                        return item;
                    })
                .ToList();
        }
        #endregion Items

        #region GameObjects
        public IEnumerable<GameObject> GetGameObjects()
        {
            var gameobjects = GetContext().gameobject_template
                .AsNoTracking()
                .Select(o => new GameObject { Id = o.entry, Name = o.name, Type = o.type });

            return gameobjects;
        }
        #endregion GameObjects
    }
}
