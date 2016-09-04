using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.Editor {

    public static class SerializedPropertyUtil {

        /// <summary> Creates an enumerator for iterating over the array elements in an array. </summary>
        /// <param name="property"> the property to enummerate </param>
        /// <returns> an enumerator for the property </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="property" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="property" /> is not an array property </exception>
        public static IEnumerable<SerializedProperty> AsArrayEnumerable(this SerializedProperty property) {
            Argument.NotNull(property);
            Argument.Check(property.isArray);
            for (var i = 0; i < property.arraySize; i++)
                yield return property.GetArrayElementAtIndex(i);
        }

        /// <summary> Adds an element to an array property at a given index and returns the property that was added. </summary>
        /// <param name="property"> the array property </param>
        /// <param name="index"> the index to insert a new element at </param>
        /// <returns> the SerializedProperty pointing to the array element </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="property" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="property" /> is not an array property or <paramref name="index" />
        /// is out of bounds </exception>
        public static SerializedProperty AddElementAtIndex(this SerializedProperty property, int index) {
            Argument.NotNull(property);
            Argument.NotNull(property.isArray);
            Argument.Check(Check.Range(index, property.arraySize + 1));
            property.InsertArrayElementAtIndex(index);
            return property.GetArrayElementAtIndex(index);
        }

        /// <summary> Adds an element to an array property at the end and returns the property that was added. </summary>
        /// <param name="property"> the array property </param>
        /// <returns> the SerializedProperty pointing to the array element </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="property" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="property" /> is not an array property. </exception>
        public static SerializedProperty Append(this SerializedProperty property) {
            return property.AddElementAtIndex(property.arraySize);
        }

        /// <summary> Adds an element to an array property at the beginning and returns the property that was added. </summary>
        /// <param name="property"> the array property </param>
        /// <returns> the SerializedProperty pointing to the array element </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="property" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="property" /> is not an array property. </exception>
        public static SerializedProperty Prepend(this SerializedProperty property) {
            return property.AddElementAtIndex(0);
        }

        /// <summary> Sets an array SerializedProperty equal to a enumerable source. </summary>
        /// <typeparam name="T"> the type of object to insert </typeparam>
        /// <param name="property"> the array property to edit </param>
        /// <param name="source"> the source collection to read values from </param>
        /// <exception cref="ArgumentNullException"> <paramref name="property" /> is null </exception>
        /// <exception cref="ArgumentException"> <paramref name="property" /> is not an array property </exception>
        public static void SetArray<T>(this SerializedProperty property, IEnumerable<T> source, int start = 0)
            where T : Object {
            Argument.NotNull(property);
            Argument.Check(property.isArray);
            int i = start;
            foreach (T obj in Argument.NotNull(source)) {
                if (i >= property.arraySize)
                    property.InsertArrayElementAtIndex(i);
                property.GetArrayElementAtIndex(i).objectReferenceValue = obj;
                i++;
            }
            property.arraySize = i;
        }

        /// <summary> Appends elements at the end of an array SerializedProperty </summary>
        /// <typeparam name="T"> the type of object to add to the array </typeparam>
        /// <param name="property"> the SerializedProperty to append onto </param>
        /// <param name="source"> the source collection to read values from </param>
        /// <exception cref="ArgumentNullException"> <paramref name="property" /> is null </exception>
        /// <exception cref="ArgumentException"> <paramref name="property" /> is not an array property </exception>
        public static void AppendArray<T>(this SerializedProperty property, IEnumerable<T> source) where T : Object {
            Argument.NotNull(property).SetArray(source, property.arraySize);
        }

    }

}