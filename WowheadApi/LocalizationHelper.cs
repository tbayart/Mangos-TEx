using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WowheadApi
{
    public static class LocalizationHelper
    {
        private static IEnumerable<string> _supportedCultures = new string[]
        {
            "de-DE",
            "en-US",
            "es-ES",
            "fr-FR",
            "pt-BR",
            "ru-RU",
        };

        public static CultureInfo DefaultCulture { get { return CultureInfo.GetCultureInfo("en-US"); } }

        public static CultureInfo[] SupportedCultures
        {
            get
            {
                return _supportedCultures
                    .Select(o => CultureInfo.GetCultureInfo(o))
                    .ToArray();
            }
        }
    }
}
