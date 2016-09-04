using System;
using System.Collections;
using System.Collections.Generic;

namespace HouraiTeahouse {

    public static class Argument {

        /// <summary> Checks if an argument is null or not. </summary>
        /// <typeparam name="T"> the type of the argument to check </typeparam>
        /// <param name="argument"> the argument itself </param>
        /// <returns> the argument, if it isn't null </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="argument" /> is null </exception>
        public static T NotNull<T>(T argument) {
            if (argument == null)
                throw new ArgumentNullException();
            return argument;
        }

        /// <summary> Performs a check on an argument, and throws a ArgumentException if it fails. </summary>
        /// <param name="check"> the check's result </param>
        /// <exception cref="ArgumentException"> <paramref name="check" /> is false </exception>
        public static void Check(bool check) {
            if (!check)
                throw new ArgumentException();
        }

        /// <summary> Performs a check on an argument, and throws a ArgumentException if it fails. </summary>
        /// <param name="name"> the name of the parameter </param>
        /// <param name="check"> the check's result </param>
        /// <exception cref="ArgumentException"> <paramref name="check" /> is false </exception>
        public static void Check(string name, bool check) {
            if (!check)
                throw new ArgumentException(name);
        }

        public static void IsGreater<T>(T a, T b) where T : IComparable<T> {
            Check(a.CompareTo(b) > 0);
        }

        public static void IsGE<T>(T a, T b) where T : IComparable<T> {
            Check(a.CompareTo(b) >= 0);
        }

        public static void IsLesser<T>(T a, T b) where T : IComparable<T> {
            Check(a.CompareTo(b) < 0);
        }

        public static void IsLE<T>(T a, T b) where T : IComparable<T> {
            Check(a.CompareTo(b) <= 0);
        }

        public static void InRange<T>(T value, T a, T b) where T : IComparable<T> {
            Check(HouraiTeahouse.Check.Range(value, a, b));
        }

        public static T NotEmpty<T>(T enumeration) where T : IEnumerable {
            Check(!enumeration.IsNullOrEmpty());
            return enumeration;
        }

    }

    public static class Check {

        /// <summary> Checks if an index is valid on a list. </summary>
        /// <typeparam name="T"> the type of trhe list </typeparam>
        /// <param name="val"> the index to check </param>
        /// <param name="list"> the list to check on </param>
        /// <returns> whether or not it's in range </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="list" /> </exception>
        public static bool Range<T>(int val, IList<T> list) { return Range(val, Argument.NotNull(list).Count); }

        /// <summary> Checks if <paramref name="val" /> is in range [<paramref name="a" />, <paramref name="b" />) </summary>
        /// <param name="val"> the value </param>
        /// <param name="a"> the lower limit, inclusive </param>
        /// <param name="b"> the upper limit, exclusive </param>
        /// <returns> whether or not it's in range </returns>
        public static bool Range<T>(T val, T a, T b) where T : IComparable<T> {
            return val.CompareTo(a) >= 0 && val.CompareTo(b) < 0;
        }

        /// <summary> Checks if <paramref name="val" /> is in range [0, <paramref name="a" />) </summary>
        /// <param name="val"> the value </param>
        /// <param name="a"> the upper limit, exclusive </param>
        /// <returns> whether or not it's in range </returns>
        public static bool Range(int val, int a) { return Range(val, 0, a); }

        /// <summary> Checks if <paramref name="val" /> is in range [0, <paramref name="a" />) </summary>
        /// <param name="val"> the value </param>
        /// <param name="a"> the upper limit, exclusive </param>
        /// <returns> whether or not it's in range </returns>
        public static bool Range(float val, float a) { return Range(val, 0f, a); }

    }

}
