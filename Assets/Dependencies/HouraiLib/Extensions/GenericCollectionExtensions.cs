using System;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse {
    /// <summary>
    /// Set of extension methods for collections and enumerations of any type.
    /// </summary>
    public static class GenericCollectionExtensions {
        /// <summary>
        /// Selects a random element from a list.
        /// </summary>
        /// <exception cref="NullReferenceException">thrown if <paramref name="list"/> is null</exception>
        /// <typeparam name="T">the type of the list</typeparam>
        /// <param name="list">the list to randomly select from</param>
        /// <returns>a random element from the list</returns>
        public static T Random<T>(this IList<T> list) {
            Check.ArgumentNull("list", list);
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Selects a random element from a list, within a specified range.
        /// </summary>
        /// <typeparam name="T">the type of the list</typeparam>
        /// <param name="list">the list to randomly select from</param>
        /// <param name="start">the start index of the range to select from. Will be clamped to [0, list.Count]</param>
        /// <param name="end">the start index of the range to select from. Will be clamped to [0, list.Count]</param>
        /// <returns>a random element from the list selected from the range</returns>
        public static T Random<T>(this IList<T> list, int start, int end) {
            Check.ArgumentNull("list", list);
            start = Mathf.Clamp(start, 0, list.Count);
            end = Mathf.Clamp(end, 0, list.Count);
            return list[UnityEngine.Random.Range(start, end)];
        }
    }
}
