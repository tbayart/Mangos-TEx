using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Framework.Collections;

namespace WowApi
{
    /// <summary>
    /// Helpers to manage cultures with WoW API
    /// </summary>
    public static class LocalizationHelper
    {
        #region Fields
        /// <summary>
        /// List of supported cultures
        /// - culture string
        /// - host to request WoW API
        /// </summary>
        private static IEnumerable<Tuple<string, string>> _localeInfoList = new TupleList<string, string>
        {
            { "en-US", "us.api.battle.net" },
            { "es-MX", "us.api.battle.net" },
            { "pt-BR", "us.api.battle.net" },
            { "en-GB", "eu.api.battle.net" },
            { "es-ES", "eu.api.battle.net" },
            { "fr-FR", "eu.api.battle.net" },
            { "ru-RU", "eu.api.battle.net" },
            { "de-DE", "eu.api.battle.net" },
            { "pl-PL", "eu.api.battle.net" },
            { "pt-PT", "eu.api.battle.net" },
            { "it-IT", "eu.api.battle.net" },
            { "ko-KR", "kr.api.battle.net" },
            { "zh-TW", "tw.api.battle.net" },
        };
        #endregion Fields

        #region Private Methods
        private static Tuple<string, string> GetItem(CultureInfo culture)
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
        /// Provide WoW API host name for a given culture
        /// </summary>
        /// <param name="culture">The requested culture</param>
        /// <returns>The woW API host string</returns>
        public static string GetHost(CultureInfo culture)
        {
            return string.Format("https://{0}/wow/", GetItem(culture).Item2);
        }
        #endregion Public Methods
    }
}
