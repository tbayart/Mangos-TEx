using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Framework.Collections;

namespace WowFramework.Helpers
{
    public static class LocaleHelpers
    {
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

        public static CultureInfo DefaultCulture { get { return CultureInfo.GetCultureInfo("en-US"); } }

        public static CultureInfo[] GetCultures()
        {
            return _localeInfoList
                .Select(o => CultureInfo.GetCultureInfo(o.Item1))
                .ToArray();
        }

        private static Tuple<string, int, string> GetItem(CultureInfo culture)
        {
            // try to locate culture
            // if not found, return default "en-US"
            return _localeInfoList.FirstOrDefault(o => o.Item1 == culture.Name)
                ?? _localeInfoList.First(o => o.Item1 == DefaultCulture.Name);
        }

        // Get locale for a given culture
        public static string GetLocale(CultureInfo culture)
        {
            // return the culture name translated to locale name
            return GetItem(culture).Item1.Replace('-', '_');
        }

        // returns the locale offset for a culture
        public static int GetOffset(CultureInfo culture)
        {
            return GetItem(culture).Item2;
        }

        // Get host name for a given culture
        public static string GetHost(CultureInfo culture)
        {
            return GetItem(culture).Item3;
        }
    }
}
