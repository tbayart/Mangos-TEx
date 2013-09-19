using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Framework.Collections;

namespace WowFramework.Helpers
{
    /// <summary>
    /// Helpers to manage cultures with mangos, WoW API and other data providers
    /// </summary>
    public static class LocaleHelpers
    {
        #region Fields
        /// <summary>
        /// List of supported cultures
        /// - culture string
        /// - culture offset for mangos
        /// - host to request WoW API
        /// </summary>
        private static IEnumerable<Tuple<string, int, string>> _localeInfoList = new TupleList<string, int, string>
        {
            { "en-US", 0, "us.battle.net" },
            { "es-MX", 7, "us.battle.net" },
            { "pt-BR", 7, "us.battle.net" },
            { "en-GB", 0, "eu.battle.net" },
            { "es-ES", 6, "eu.battle.net" },
            { "fr-FR", 2, "eu.battle.net" },
            { "ru-RU", 8, "eu.battle.net" },
            { "de-DE", 3, "eu.battle.net" },
            { "pt-PT", 6, "eu.battle.net" },
            { "it-IT", 0, "eu.battle.net" },
            { "ko-KR", 1, "kr.battle.net" },
            { "zh-TW", 5, "tw.battle.net" },
            { "zh-CN", 4, "www.battlenet.com.cn" },
        };
        #endregion Fields

        #region Private Methods
        private static Tuple<string, int, string> GetItem(CultureInfo culture)
        {
            culture = ValidateCulture(culture);
            // locate the culture
            return _localeInfoList.First(o => o.Item1 == culture.Name);
        }
        #endregion Private Methods

        #region Public Methods
        /// <summary>
        /// The default culture
        /// </summary>
        public static CultureInfo DefaultCulture { get { return CultureInfo.GetCultureInfo("en-US"); } }

        /// <summary>
        /// Validates a culture by returning a supported culture for a given one
        /// Returns the provided culture if supported
        /// Else returns the default supported culture
        /// </summary>
        /// <param name="culture">The requested culture</param>
        /// <returns>A supported culture</returns>
        public static CultureInfo ValidateCulture(CultureInfo culture)
        {
            return _localeInfoList.Any(o => o.Item1 == culture.Name)
                ? culture
                : DefaultCulture;
        }

        /// <summary>
        /// Give access to all supported cultures
        /// </summary>
        /// <returns>An array of supported CultureInfo</returns>
        public static CultureInfo[] GetCultures()
        {
            return _localeInfoList
                .Select(o => CultureInfo.GetCultureInfo(o.Item1))
                .ToArray();
        }

        /// <summary>
        /// Provide WoW API locale for a given culture
        /// </summary>
        /// <param name="culture">The requested culture</param>
        /// <returns></returns>
        public static string GetLocale(CultureInfo culture)
        {
            // return the culture name translated to locale name
            return GetItem(culture).Item1.Replace('-', '_');
        }

        /// <summary>
        /// Provide the locale offset for a culture
        /// </summary>
        /// <param name="culture">The requested culture</param>
        /// <returns>The culture offset</returns>
        public static int GetOffset(CultureInfo culture)
        {
            return GetItem(culture).Item2;
        }

        /// <summary>
        /// Provide WoW API host name for a given culture
        /// </summary>
        /// <param name="culture">The requested culture</param>
        /// <returns>The woW API host string</returns>
        public static string GetHost(CultureInfo culture)
        {
            return string.Format("https://{0}/api/wow/", GetItem(culture).Item3);
        }
        #endregion Public Methods
    }
}
