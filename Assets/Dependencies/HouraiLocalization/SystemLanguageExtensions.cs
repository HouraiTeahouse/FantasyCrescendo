// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Globalization;
using UnityEngine;

namespace HouraiTeahouse.Localization {
    public static class SystemLanguageExtensions {
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
                    return
                        systemLanguage.ToString()
                            .Substring(0, 3)
                            .Remove(1, 1)
                            .ToLower();
                case SystemLanguage.Faroese:
                case SystemLanguage.Latvian:
                case SystemLanguage.Portuguese:
                case SystemLanguage.Bulgarian:
                    return
                        systemLanguage.ToString()
                            .Substring(0, 4)
                            .Remove(1, 2)
                            .ToLower();
            }
            return systemLanguage.ToString().Substring(0, 2).ToLowerInvariant();
        }

        /// <summary> Converts a SystemLanugage value into a CultureInfo. </summary>
        /// <param name="systemLanguage"> the SystemLanugage value to map </param>
        /// <returns> the corresponding CultureInfo </returns>
        public static CultureInfo ToCultureInfo(
            this SystemLanguage systemLanguage) {
            if (systemLanguage == SystemLanguage.Unknown)
                return CultureInfo.InvariantCulture;
            return new CultureInfo(systemLanguage.ToIdentifier());
        }
    }
}