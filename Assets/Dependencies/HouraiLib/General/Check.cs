using System;
using System.Collections;
using System.Collections.Generic;

namespace HouraiTeahouse {

    public static class Check {

        /// <summary>
        /// Checks if an argument is null or not.
        /// </summary>
        /// <typeparam name="T">the type of the argument to check</typeparam>
        /// <param name="name">the name of the argument for the exception</param>
        /// <param name="argument">the argument itself</param>
        /// <returns>the argument, if it isn't null</returns>
        /// <exception cref="ArgumentNullException"><paramref name="argument"/> is null</exception>
        public static T NotNull<T>(string name, T argument) {
            if (argument == null)
                throw new ArgumentNullException(name);
            return argument;
        }

        /// <summary>
        /// Performs a check on an argument, and throws a ArgumentException if it fails.
        /// </summary>
        /// <param name="check">the check's result</param>
        /// <exception cref="ArgumentException"><paramref name="check"/> is false</exception>
        public static void Argument(bool check) {
            if (!check)
                throw new ArgumentException();
        }

        /// <summary>
        /// Performs a check on an argument, and throws a ArgumentException if it fails.
        /// </summary>
        /// <param name="name">the name of the parameter</param>
        /// <param name="check">the check's result</param>
        /// <exception cref="ArgumentException"><paramref name="check"/> is false</exception>
        public static void Argument(string name, bool check) {
            if (!check)
                throw new ArgumentException(name);
        }

        /// <summary>
        /// Checks if an index is valid on a list.
        /// </summary>
        /// <typeparam name="T">the type of trhe list</typeparam>
        /// <param name="val">the index to check</param>
        /// <param name="list">the list to check on</param>
        /// <returns>whether or not it's in range</returns>
        /// <exception cref="ArgumentNullException"><paramref name="list"/></exception>
        public static bool Range<T>(int val, IList<T> list) {
            return Range(val, NotNull("list", list).Count);
        }

        /// <summary>
        /// Checks if <paramref name="val"/> is in range [<paramref name="a"/>, <paramref name="b"/>)
        /// </summary>
        /// <param name="val">the value</param>
        /// <param name="a">the lower limit, inclusive</param>
        /// <param name="b">the upper limit, exclusive</param>
        /// <returns>whether or not it's in range</returns>
        public static bool Range(int val, int a, int b) {
            return val >= a && val < b;
        }

        /// <summary>
        /// Checks if <paramref name="val"/> is in range [<paramref name="a"/>, <paramref name="b"/>)
        /// </summary>
        /// <param name="val">the value</param>
        /// <param name="a">the lower limit, inclusive</param>
        /// <param name="b">the upper limit, exclusive</param>
        /// <returns>whether or not it's in range</returns>
        public static bool Range(float val, float a, float b) {
            return val >= a && val < b;
        }

        /// <summary>
        /// Checks if <paramref name="val"/> is in range [0, <paramref name="a"/>)
        /// </summary>
        /// <param name="val">the value</param>
        /// <param name="a">the upper limit, exclusive</param>
        /// <returns>whether or not it's in range</returns>
        public static bool Range(int val, int a) {
            return Range(val, 0, a);
        }

        /// <summary>
        /// Checks if <paramref name="val"/> is in range [0, <paramref name="a"/>)
        /// </summary>
        /// <param name="val">the value</param>
        /// <param name="a">the upper limit, exclusive</param>
        /// <returns>whether or not it's in range</returns>
        public static bool Range(float val, float a) {
            return Range(val, 0f, a);
        }
        /// <summary>
        /// Check if an enumeration is empty, null, or not. Throws an InvalidOperationException if it is.
        /// </summary>
        /// <typeparam name="T">the enumeration to check</typeparam>
        /// <param name="enumeration">the enumeration to check</param>
        /// <exception cref="InvalidOperationException"><paramref name="enumeration"/> is empty</exception>
        /// <returns>the enumeration, if it isn't empty</returns>
        public static T NotEmpty<T>(T enumeration) where T : IEnumerable {
            if(enumeration.IsNullOrEmpty())
                throw new InvalidOperationException();
            return enumeration;
        }

        /// <summary>
        /// Generic version of NotEmpty. Throws an exception of a certain type if <paramref name="enumeration"/>
        /// is empty or null.
        /// </summary>
        /// <typeparam name="T">the type of exception to throw</typeparam>
        /// <param name="enumeration">the enumeration to check</param>
        public static void NotEmpty<T>(IEnumerable enumeration) where T : Exception, new() {
            if(enumeration.IsNullOrEmpty())
                throw new T();
        }

    }

}
