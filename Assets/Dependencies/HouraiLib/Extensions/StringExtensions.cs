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

using System;

namespace HouraiTeahouse {
    public static class StringExtensions {
        /// <summary> Generic function that will parse a string into an enum value. </summary>
        /// <typeparam name="T"> the type of enum to parse into </typeparam>
        /// <param name="str"> the string to parse </param>
        /// <param name="ignoreCase"> whether or not to ignore case when parsing </param>
        /// <exception cref="ArgumentNullException"> <paramref name="str" /> is null </exception>
        /// <returns> the enum value </returns>
        public static T ToEnum<T>(this string str, bool ignoreCase = false)
            where T : struct, IComparable, IFormattable, IConvertible {
            Check.Argument(str.IsNullOrEmpty());
            return
                (T) Enum.Parse(typeof(T), Check.NotNull("str", str), ignoreCase);
        }

        /// <summary> Shorthand for string.With. </summary>
        /// <param name="str"> string to format </param>
        /// <param name="objs"> the objects to format it with </param>
        /// <exception cref="ArgumentNullException"> <paramref name="str" /> or <paramref name="objs" /> is null </exception>
        /// <returns> the formatted string </returns>
        public static string With(this string str, params object[] objs) {
            return string.Format(Check.NotNull("str", str),
                Check.NotNull("objs", objs));
        }

        public static string EmptyIfNull(this string str) {
            return str ?? string.Empty;
        }
    }
}