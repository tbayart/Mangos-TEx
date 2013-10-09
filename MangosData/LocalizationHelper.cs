using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Framework.Collections;

namespace MangosData
{
    /// <summary>
    /// Helpers to manage cultures with mangos
    /// </summary>
    public static class LocalizationHelper
    {
        #region Fields
        /// <summary>
        /// List of supported cultures
        /// - culture string
        /// - culture offset for mangos
        /// - host to request WoW API
        /// </summary>
        private static IEnumerable<Tuple<string, int>> _localeInfoList = new TupleList<string, int>
        {
            { "en-US", 0 },
            { "ko-KR", 1 },
            { "fr-FR", 2 },
            { "de-DE", 3 },
            { "zh-CN", 4 },
            { "zh-TW", 5 },
            { "es-ES", 6 },
            { "es-MX", 7 },
            { "ru-RU", 8 },
        };
        #endregion Fields

        #region Private Methods
        private static Tuple<string, int> GetItem(CultureInfo culture)
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
        /// Provide the locale offset for a culture
        /// </summary>
        /// <param name="culture">The requested culture</param>
        /// <returns>The culture offset</returns>
        public static int GetOffset(CultureInfo culture)
        {
            return GetItem(culture).Item2;
        }
        #endregion Public Methods
    }
}
