using System;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using ApiCommon;
using Framework.Helpers;
using Framework.Interfaces;
using MangosTEx.Services.Models;
using MangosTExData.Context;
using MySql.Data.MySqlClient;

namespace MangosTEx.Services.ApiDataProvider
{
    internal class HttpCachedProvider : IDataProvider
    {
        #region Fields
        private IDataProvider _httpProvider;
        #endregion Fields

        #region Ctor
        public HttpCachedProvider()
        {
            _httpProvider = DataProviderManager.GetSimpleHttpProvider();
        }
        #endregion Ctor

        #region Methods
        private static MangosTExEntities GetContext(uint timeout = 15)
        {
            // retrieve entity ConnectionString from configuration file
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["MangosTExEntities"];
            // parsing entity ConnectionString
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder(settings.ConnectionString);
            // using settings to create a ProviderConnectionString for MySql
            MySqlConnectionStringBuilder mysqlBuilder = new MySqlConnectionStringBuilder();
            mysqlBuilder.ConnectionTimeout = timeout;
            //mysqlBuilder.Server = Settings.DatabaseHost;
            //mysqlBuilder.Port = Settings.DatabasePort;

            // setting up ProviderConnectionString in entity ConnectionString
            entityBuilder.ProviderConnectionString = mysqlBuilder.ConnectionString;
            // create the DbContext using ConnectionString
            return new MangosTExEntities(entityBuilder.ConnectionString);
        }

        private dataprovidercache CacheData(string source)
        {
            string data = WebRequestHelpers.DownloadString(source);
            return new dataprovidercache(source, data);
        }
        #endregion Methods

        #region IDataProvider Members
        public string GetProviderName()
        {
            return "HTTP Cached Provider";
        }

        public IStatusMessage GetStatus()
        {
            try
            {
                using (var context = GetContext(3))
                {
                    var adapter = (System.Data.Entity.Infrastructure.IObjectContextAdapter)context;
                    if (adapter.ObjectContext.DatabaseExists() == false)
                        return new ConnectionStatus(false, "Failed to access database");
                }
            }
            catch (Exception ex)
            {
                return new ConnectionStatus(false, ex.Message);
            }
            return _httpProvider.GetStatus();
        }

        public string ProvideData(string source)
        {
            using (var provider = new MangosTExEntities())
            {
                var cachedData = provider.dataprovidercache.AsNoTracking()
                    .FirstOrDefault(o => o.source == source);

                if (cachedData == null)
                {
                    cachedData = CacheData(source);
                    provider.dataprovidercache.Add(cachedData);
                    provider.SaveChanges();
                }
                return cachedData.data;
            }
        }
        #endregion IDataProvider Members
    }
}
