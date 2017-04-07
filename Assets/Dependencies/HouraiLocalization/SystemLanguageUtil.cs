using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.Localization {

    public static class SystemLanguageUtil {

        static readonly Dictionary<string, SystemLanguage> _inverse;

        static SystemLanguageUtil() {
            _inverse = new Dictionary<string, SystemLanguage>();
            foreach (var language in Enum.GetValues(typeof(SystemLanguage)).OfType<SystemLanguage>())
                _inverse[ToIdentifier(language)] = language;
        }

        public static SystemLanguage ToLanguage(string identifier) {
            SystemLanguage value;
            if (_inverse.TryGetValue(identifier, out value))
                return value;
            return SystemLanguage.Unknown;
        }

        public static SystemLanguage ToLanguage(CultureInfo info) {
            if (info == null)
                return SystemLanguage.Unknown;
            return ToLanguage(info.ToString());
        }

        public static string ToIdentifier(this SystemLanguage systemLanguage) {
            switch (systemLanguage) {
                case SystemLanguage.Basque:
                    return "eu";
                case SystemLanguage.Chinese:
                    return "zh-cn";
                case SystemLanguage.ChineseSimplified:
                    return "zh-chs";
                case SystemLanguage.ChineseTraditional:
                    return "zh-cht";
                case SystemLanguage.Czech:
                    return "cs";
                case SystemLanguage.Dutch:
                    return "nl";
                case SystemLanguage.Spanish:
                    return "es";
                case SystemLanguage.SerboCroatian:
                    return "hr";
                case SystemLanguage.Swedish:
                    return "sv";
                case SystemLanguage.German:
                    return "de";
                case SystemLanguage.Greek:
                    return "el";
                case SystemLanguage.Icelandic:
                    return "is";
                case SystemLanguage.Slovak:
                    return "sk";
                case SystemLanguage.Estonian:
                case SystemLanguage.Indonesian:
                case SystemLanguage.Lithuanian:
                case SystemLanguage.Polish:
                case SystemLanguage.Turkish:
                    return systemLanguage.ToString().Substring(0, 3).Remove(1, 1).ToLower();
                case SystemLanguage.Faroese:
                case SystemLanguage.Latvian:
                case SystemLanguage.Portuguese:
                case SystemLanguage.Bulgarian:
                    return systemLanguage.ToString().Substring(0, 4).Remove(1, 2).ToLower();
            }
            return systemLanguage.ToString().Substring(0, 2).ToLowerInvariant();
        }

        /// <summary> Converts a SystemLanugage value into a CultureInfo. </summary>
        /// <param name="systemLanguage"> the SystemLanugage value to map </param>
        /// <returns> the corresponding CultureInfo </returns>
        public static CultureInfo ToCultureInfo(this SystemLanguage systemLanguage) {
            if (systemLanguage == SystemLanguage.Unknown)
                return CultureInfo.InvariantCulture;
            return new CultureInfo(systemLanguage.ToIdentifier());
        }

    }

}
