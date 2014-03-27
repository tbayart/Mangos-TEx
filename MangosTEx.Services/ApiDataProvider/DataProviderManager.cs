using ApiCommon;

namespace MangosTEx.Services.ApiDataProvider
{
    public static class DataProviderManager
    {
        public static IDataProvider GetSimpleHttpProvider()
        {
            return new SimpleHttpProvider();
        }

        public static IDataProvider GetHttpCachedProvider()
        {
            var provider = new HttpCachedProvider();

            // check database access
            //var c = new MySql.Data.MySqlClient.MySqlConnection(

            // create mangostex user if not exists :
            //      CREATE USER 'mangostex'@'localhost' IDENTIFIED BY 'mangostex';

            // create mangostex database if not exists :
            //      CREATE DATABASE IF NOT EXISTS `mangostex` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;

            // grant mangostex user access to mangostex database :
            //      GRANT ALL PRIVILEGES ON `mangostex` . * TO 'mangostex'@'localhost' WITH GRANT OPTION;

            // create provider table :
            //      CREATE TABLE `dataprovidercache` (
            //          `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
            //          `source` varchar(100) NOT NULL,
            //          `date` datetime NOT NULL,
            //          `data` mediumtext NOT NULL,
            //          PRIMARY KEY (`id`),
            //          UNIQUE KEY `id_UNIQUE` (`id`)
            //      ) ENGINE=MyISAM DEFAULT CHARSET=utf8 ROW_FORMAT=FIXED;

            // instanciate provider

            // return provider instance
            return provider;
        }
    }
}
