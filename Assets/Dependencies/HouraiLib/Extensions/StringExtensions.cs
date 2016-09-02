using System;

namespace HouraiTeahouse {

    public static class StringExtensions {

        public static string OptionalFormat(this string format, params object[] parameters) {
            return !string.IsNullOrEmpty(format) ? string.Format(format, parameters) : format;
        }

        /// <summary> Generic function that will parse a string into an enum value. </summary>
        /// <typeparam name="T"> the type of enum to parse into </typeparam>
        /// <param name="str"> the string to parse </param>
        /// <param name="ignoreCase"> whether or not to ignore case when parsing </param>
        /// <exception cref="ArgumentNullException"> <paramref name="str" /> is null </exception>
        /// <returns> the enum value </returns>
        public static T ToEnum<T>(this string str, bool ignoreCase = false)
            where T : struct, IComparable, IFormattable, IConvertible {
            Check.Argument(str.IsNullOrEmpty());
            return (T) Enum.Parse(typeof(T), Check.NotNull(str), ignoreCase);
        }

        /// <summary> Shorthand for string.With. </summary>
        /// <param name="str"> string to format </param>
        /// <param name="objs"> the objects to format it with </param>
        /// <exception cref="ArgumentNullException"> <paramref name="str" /> or <paramref name="objs" /> is null </exception>
        /// <returns> the formatted string </returns>
        public static string With(this string str, params object[] objs) {
            return string.Format(Check.NotNull(str), objs);
        }

        public static string EmptyIfNull(this string str) { return str ?? string.Empty; }

    }

}