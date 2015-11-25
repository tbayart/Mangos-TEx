using System;
using System.Collections.Generic;
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
        public void InitProvider()
        {
            var status = GetStatus();
            if (status.IsOk == false)
            {
                var ecs = GetEntityConnectionString();
                var rootAccess = new MySqlConnectionStringBuilder(ecs.ProviderConnectionString);
                rootAccess.Database = "";
                rootAccess.UserID = "root"; // parameters["userid"]
                rootAccess.Password = ""; // parameters["userpwd"]
                CreateDatabase(rootAccess.ConnectionString);
            }

            using (var context = GetContext())
                context.dataprovidercache.FirstOrDefault();
        }

        private void CreateDatabase(string connectionString)
        {
            string[] database_commands = {
                // create mangostex database if not exists
                "CREATE DATABASE IF NOT EXISTS mangostex DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;",
                // switch to created database
                "use mangostex",
                // create mangostex user if not exists and/or grant required privileges
                "GRANT INSERT, SELECT, UPDATE, DELETE ON mangostex.* TO 'mangostex'@'localhost' IDENTIFIED BY 'mangostex';",
                // allow mangostex user on localhost
                "GRANT PROXY ON 'mangostex'@'%' TO 'mangostex'@'localhost';",
                // force cached privileges to be updated
                "FLUSH PRIVILEGES;",
                // create cache table
                "CREATE TABLE IF NOT EXISTS dataprovidercache (id int(11) unsigned NOT NULL AUTO_INCREMENT, source varchar(100) NOT NULL, date datetime NOT NULL, data mediumtext NOT NULL, PRIMARY KEY (id), UNIQUE KEY id_UNIQUE (id)) ENGINE=MyISAM DEFAULT CHARSET=utf8 ROW_FORMAT=FIXED;",
            };

            // check database access
            using (var cnx = new MySqlConnection(connectionString))
            {
                cnx.Open();
                //if(conn.State != System.Data.ConnectionState.Open)
                database_commands.ForEach(cmd => Execute(cmd, cnx));
                cnx.Close();
            }
        }

        private static void Execute(string query, MySqlConnection cnx)
        {
            using (var cmd = new MySqlCommand(query, cnx))
            {
                int result = cmd.ExecuteNonQuery();
                Console.WriteLine(result);
            }
        }

        private static MangosTExEntities GetContext(uint timeout = 15)
        {
            // getting entity ConnectionString
            EntityConnectionStringBuilder entityBuilder = GetEntityConnectionString();
            // using settings to create a ProviderConnectionString for MySql
            MySqlConnectionStringBuilder mysqlBuilder = new MySqlConnectionStringBuilder(entityBuilder.ProviderConnectionString);
            mysqlBuilder.ConnectionTimeout = timeout;
            //mysqlBuilder.Server = Settings.DatabaseHost;
            //mysqlBuilder.Port = Settings.DatabasePort;

            // setting up ProviderConnectionString in entity ConnectionString
            entityBuilder.ProviderConnectionString = mysqlBuilder.ConnectionString;
            // create the DbContext using ConnectionString
            return new MangosTExEntities(entityBuilder.ConnectionString);
        }

        private static EntityConnectionStringBuilder GetEntityConnectionString()
        {
            // retrieve entity ConnectionString from configuration file
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["MangosTExEntities"];
            return new EntityConnectionStringBuilder(settings.ConnectionString);
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
