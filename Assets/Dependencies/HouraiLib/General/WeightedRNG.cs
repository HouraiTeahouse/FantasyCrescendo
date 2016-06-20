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
using Random = UnityEngine.Random;

namespace HouraiTeahouse {
    /// <summary> A collection of objects/values </summary>
    /// <typeparam name="T"> </typeparam>
    public class WeightedRNG<T> : ICollection<T> {
        // The backing dictionary
        readonly Dictionary<T, float> _weights;

        // The sum of the weights of all elements stored in the WeightedRNG
        float _weightSum;

        /// <summary> Initializes a new empty instance of WeightedRNG. </summary>
        public WeightedRNG() {
            _weights = new Dictionary<T, float>();
        }

        /// <summary> Initializes a new WeightedRNG instance from a given collection. </summary>
        /// <remarks> Every element has the same weight of 1. If <paramref name="collection" /> is null, the resultant WeightedRNG
        /// instance will be empty. </remarks>
        /// <param name="collection"> </param>
        public WeightedRNG(IEnumerable<T> collection) : this() {
            if (collection == null)
                return;

            foreach (T element in collection)
                this[element] = 1f;
        }

        /// <summary> Initializes a new instance of WeightedRNG using the weights and elements specifed by a dictionary. </summary>
        /// <remarks> If <paramref name="dictionary" /> is null, the resultant WeightedRNG instance will be empty. </remarks>
        /// <param name="dictionary"> a map between. </param>
        public WeightedRNG(IDictionary<T, float> dictionary) : this() {
            if (dictionary == null)
                return;

            foreach (KeyValuePair<T, float> element in dictionary)
                this[element.Key] = element.Value;
        }

        /// <summary> Indexer to get or set the weight of an element. When using the setter, if the WeigthedRNG does not contain
        /// the element, it will be added. </summary>
        /// <param name="index"> the element to edit/retrieve the weight of </param>
        /// <returns> the weight of the element </returns>
        public float this[T index] {
            get { return _weights.ContainsKey(index) ? _weights[index] : 0f; }
            set {
                if (Contains(index))
                    _weightSum -= _weights[index];
                _weights[index] = value;
                _weightSum += value;
            }
        }

        /// <summary> Returns a randomly selected element from the WeightedRNG. </summary>
        /// <remarks> The probability that each element is selected is based on the relative weights of each element. Those with
        /// higher weights have porportionally higher probability of being selected. This operation runs in worst case O(n) time,
        /// where n is the Count of elements in the WeightedRNG. </remarks>
        /// <exception cref="InvalidOperationException"> the WeightedRNG is empty </exception>
        /// <returns> a randomly selected </returns>
        public T Select() {
            if (Count <= 0)
                throw new InvalidOperationException();
            float randomValue = Random.value * _weightSum;
            foreach (KeyValuePair<T, float> element in _weights) {
                randomValue -= element.Value;
                if (randomValue <= 0)
                    return element.Key;
            }
            return default(T);
        }

        #region ICollection Implemenation

        /// <summary> Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an
        /// <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index. </summary>
        /// <param name="array"> The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied
        /// from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based
        /// indexing. </param>
        /// <param name="arrayIndex"> The zero-based index in <paramref name="array" /> at which copying begins. </param>
        /// <exception cref="T:System.ArgumentNullException"> <paramref name="array" /> is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"> <paramref name="arrayIndex" /> is less than 0. </exception>
        /// <exception cref="T:System.ArgumentException"> The number of elements in the source
        /// <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from
        /// <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />. </exception>
        public void CopyTo(T[] array, int arrayIndex) {
            _weights.Keys.CopyTo(array, arrayIndex);
        }

        /// <summary> Removes the first occurrence of a specific object from the
        /// <see cref="T:System.Collections.Generic.ICollection`1" />. </summary>
        /// <returns> true if <paramref name="item" /> was successfully removed from the
        /// <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if
        /// <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />. </returns>
        /// <param name="item"> The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />. </param>
        /// <exception cref="T:System.NotSupportedException"> The <see cref="T:System.Collections.Generic.ICollection`1" /> is
        /// read-only. </exception>
        public bool Remove(T obj) {
            if (!_weights.ContainsKey(obj))
                return false;

            bool success = _weights.Remove(obj);
            if (success)
                _weightSum -= _weights[obj];
            return success;
        }

        /// <summary> A count of how many elements are in the WeightedRNG. </summary>
        public int Count {
            get { return _weights.Count; }
        }

        /// <summary> Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </summary>
        /// <returns> true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false. </returns>
        public bool IsReadOnly {
            get { return false; }
        }

        /// <summary> Adds an item to the WeightedRNG with a weight of 1. </summary>
        /// <param name="item"> </param>
        public void Add(T item) {
            this[item] = 1;
        }

        /// <summary> Clears all elements from the WeightedRNG </summary>
        public void Clear() {
            _weights.Clear();
            _weightSum = 0;
        }

        /// <summary> Checks if the WeightedRNG contains a certain element. </summary>
        /// <param name="obj"> the element to check for </param>
        /// <returns> whether the element is </returns>
        public bool Contains(T obj) {
            return _weights.ContainsKey(obj);
        }

        /// <summary> Returns an enumerator that iterates through the collection. </summary>
        /// <returns> An enumerator that can be used to iterate through the collection. </returns>
        /// <filterpriority> 1 </filterpriority>
        public IEnumerator<T> GetEnumerator() {
            return _weights.Keys.GetEnumerator();
        }

        /// <summary> Returns an enumerator that iterates through a collection. </summary>
        /// <returns> An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection. </returns>
        /// <filterpriority> 2 </filterpriority>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }
}