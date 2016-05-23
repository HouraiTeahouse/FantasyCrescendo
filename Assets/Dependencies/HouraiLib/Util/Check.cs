using System;
using System.Collections.Generic;

namespace HouraiTeahouse {

    public static class Check {

        // Performs a null check and throws a ArgumentNullException if it is null
        // and returns the original object otherwise.
        public static T NotNull<T>(string name, T argument) {
            if (argument == null)
                throw new ArgumentNullException(name);
            return argument;
        }

        public static void Argument(bool check) {
            if (!check)
                throw new ArgumentException();
        }

        public static void Argument(string name, bool check) {
            if (!check)
                throw new ArgumentException(name);
        }

        // Checks if $val is in the range [0, $list.Count)
        public static bool Range<T>(int val, IList<T> list) {
            return Range(val, list.Count);
        }

        // Checks if $val is in the range [$a, $b)
        public static bool Range(int val, int a, int b) {
            return val >= a && val < b;
        }

        // Checks if $val is in the range [$a, $b)
        public static bool Range(float val, float a, float b) {
            return val >= a && val < b;
        }

        // Checks if $val is in the range [0, $a)
        public static bool Range(int val, int a) {
            return Range(val, 0, a);
        }

        // Checks if $val is in the range [0, $a)
        public static bool Range(float val, float a) {
            return Range(val, 0f, a);
        }

    }

}
