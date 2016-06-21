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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse {
    /// <summary> Set of extension methods for collections and enumerations of any type. </summary>
    public static class GenericCollectionExtensions {
        /// <summary> Checks if the enumeration is null or empty. </summary>
        /// <param name="enumeration"> the enumeration of values </param>
        /// <returns> true if <paramref name="enumeration" /> is null or empty. </returns>
        public static bool IsNullOrEmpty(this IEnumerable enumeration) {
            if (enumeration is string)
                return string.IsNullOrEmpty(enumeration as string);
            return enumeration == null || IsEmpty(enumeration);
        }

        /// <summary> Checks if a enumeration is empty or not. </summary>
        /// <param name="enumeration"> the enumeration of values </param>
        /// <exception cref="ArgumentNullException"> <paramref name="enumeration" /> is null </exception>
        /// <returns> true if <paramref name="enumeration" /> is empty, false otherwise </returns>
        public static bool IsEmpty(this IEnumerable enumeration) {
            Check.NotNull(enumeration);
            var collection = enumeration as ICollection;
            if (collection != null)
                return collection.Count <= 0;
            return !enumeration.Cast<object>().Any();
        }

        /// <summary> Samples one every N elements from an enumeration. </summary>
        /// <typeparam name="T"> the type of values being enumerated </typeparam>
        /// <param name="enumeration"> the enumeration of values </param>
        /// <param name="count"> the subsampling rate </param>
        /// <exception cref="ArgumentNullException"> <paramref name="enumeration" /> is null </exception>
        /// <returns> the subsampled enumeration </returns>
        public static IEnumerable<T> SampleEvery<T>(
            this IEnumerable<T> enumeration,
            int count) {
            if (enumeration == null)
                yield break;
            int i = count;
            foreach (T val in enumeration) {
                if (i >= count) {
                    yield return val;
                    i = 0;
                }
                i++;
            }
        }

        /// <summary> Creates an empty enumeration if the provided one is null. Used to avoid NullReferenceExceptions. </summary>
        /// <typeparam name="T"> the type of values being enumerated </typeparam>
        /// <param name="enumeration"> the enumeration of values </param>
        /// <returns> the same enumeration if it is not null, an empty enumeration if it is. </returns>
        public static IEnumerable<T> EmptyIfNull<T>(
            this IEnumerable<T> enumeration) {
            return enumeration ?? Enumerable.Empty<T>();
        }

        /// <summary> Removes all null values from an enumeration. </summary>
        /// <typeparam name="T"> the type of values being enumerated </typeparam>
        /// <param name="enumeration"> the enumeration of values </param>
        /// <returns> the enumeration without null values, empty if <paramref name="enumeration" /> is null. </returns>
        public static IEnumerable<T> IgnoreNulls<T>(
            this IEnumerable<T> enumeration) where T : class {
            return enumeration.EmptyIfNull().Where(obj => obj != null);
        }

        /// <summary> Finds the key with the maximum value over an enumeration of key-value pairs. Note this is not a streaming
        /// operator. An infinite enumeration will infinitely loop. </summary>
        /// <typeparam name="K"> the type of of the keys </typeparam>
        /// <typeparam name="V"> the type of the values </typeparam>
        /// <param name="values"> the enumeration </param>
        /// <exception cref="ArgumentNullException"> <paramref name="values" /> is null </exception>
        /// <returns> the key of the maximum value </returns>
        public static K ArgMax<K, V>(this IEnumerable<KeyValuePair<K, V>> values)
            where V : IComparable<V> {
            return FindArg(Check.NotNull(values),
                (v1, v2) => v1.CompareTo(v2) > 0);
        }

        /// <summary> Finds the key with the minimum value over an enumeration of key-value pairs. Note this is not a streaming
        /// operator. An infinite enumeration will infinitely loop. </summary>
        /// <typeparam name="K"> the type of of the keys </typeparam>
        /// <typeparam name="V"> the type of the values </typeparam>
        /// <param name="values"> the enumeration </param>
        /// <exception cref="ArgumentNullException"> <paramref name="values" /> is null </exception>
        /// <returns> the key of the minimum value </returns>
        public static K ArgMin<K, V>(this IEnumerable<KeyValuePair<K, V>> values)
            where V : IComparable<V> {
            return FindArg(Check.NotNull(values),
                (v1, v2) => v1.CompareTo(v2) < 0);
        }

        static K FindArg<K, V>(IEnumerable<KeyValuePair<K, V>> values,
                               Func<V, V, bool> func) {
            IEnumerator<KeyValuePair<K, V>> iterator = values.GetEnumerator();
            if (!iterator.MoveNext())
                throw new InvalidOperationException();
            K key = iterator.Current.Key;
            V val = iterator.Current.Value;
            while (iterator.MoveNext()) {
                if (!func(iterator.Current.Value, val))
                    continue;
                key = iterator.Current.Key;
                val = iterator.Current.Value;
            }
            return key;
        }

        /// <summary> Finds the index of the maximum value over an enumeration. Note this is not a streaming operator. An infinite
        /// enumeration will infinitely loop. </summary>
        /// <typeparam name="T"> the type of the values being enumerated </typeparam>
        /// <param name="values"> the enumeration </param>
        /// <exception cref="ArgumentNullException"> <paramref name="values" /> is null </exception>
        /// <returns> the index of the maximum value </returns>
        public static int ArgMax<T>(this IEnumerable<T> values)
            where T : IComparable<T> {
            return FindIndex(Check.NotNull(values),
                (v1, v2) => v1.CompareTo(v2) > 0);
        }

        /// <summary> Finds the index of the minimum value over an enumeration. Note this is not a streaming operator. An infinite
        /// enumeration will infinitely loop. </summary>
        /// <typeparam name="T"> the type of the values being enumerated </typeparam>
        /// <param name="values"> the enumeration </param>
        /// <exception cref="ArgumentNullException"> <paramref name="values" /> is null </exception>
        /// <returns> the index of the minimum value </returns>
        public static int ArgMin<T>(this IEnumerable<T> values)
            where T : IComparable<T> {
            return FindIndex(Check.NotNull(values),
                (v1, v2) => v1.CompareTo(v2) < 0);
        }

        static int FindIndex<T>(IEnumerable<T> enumeration,
                                Func<T, T, bool> func) {
            IEnumerator<T> iterator = enumeration.GetEnumerator();
            if (!iterator.MoveNext())
                throw new InvalidOperationException();
            var index = 0;
            T val = iterator.Current;
            var i = 0;
            while (iterator.MoveNext()) {
                i++;
                if (!func(iterator.Current, val))
                    continue;
                index = i;
                val = iterator.Current;
            }
            return index;
        }

        /// <summary> Selects a random element from a list. </summary>
        /// <typeparam name="T"> the type of the list </typeparam>
        /// <param name="list"> the list to randomly select from </param>
        /// <exception cref="ArgumentNullException"> <paramref name="list" /> is null </exception>
        /// <returns> a random element from the list </returns>
        public static T Random<T>(this IList<T> list) {
            Check.NotNull(list);
            return list.Random(0, list.Count);
        }

        /// <summary> Selects a random element from a list, within a specified range. </summary>
        /// <typeparam name="T"> the type of the list </typeparam>
        /// <param name="list"> the list to randomly select from </param>
        /// <param name="start"> the start index of the range to select from. Will be clamped to [0, list.Count] </param>
        /// <param name="end"> the start index of the range to select from. Will be clamped to [0, list.Count] </param>
        /// <exception cref="ArgumentNullException"> <paramref name="list" /> is null </exception>
        /// <returns> a random element from the list selected from the range </returns>
        public static T Random<T>(this IList<T> list, int start, int end) {
            Check.NotNull(list);
            start = Mathf.Clamp(start, 0, list.Count);
            end = Mathf.Clamp(end, 0, list.Count);
            return list[UnityEngine.Random.Range(start, end)];
        }
    }
}
