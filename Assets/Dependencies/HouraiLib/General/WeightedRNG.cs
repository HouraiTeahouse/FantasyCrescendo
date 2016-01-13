using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Hourai {

    /// <summary>
    /// A collection of objects/values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeightedRNG<T> : ICollection<T> {

        // The backing dictionary
        private readonly Dictionary<T, float> _weights;

        // The sum of the weights of all elements stored in the WeightedRNG
        private float _weightSum;

        /// <summary>
        /// Initializes a new empty instance of WeightedRNG.
        /// </summary>
        public WeightedRNG() {
            _weights = new Dictionary<T, float>();
        }

        /// <summary>
        /// Initializes a new WeightedRNG instance from a given collection.
        /// </summary>
        /// <remarks>
        /// Every element has the same weight of 1.
        /// If <paramref name="collection"/> is null, the resultant WeightedRNG instance will be empty.
        /// </remarks>
        /// <param name="collection"></param>
        public WeightedRNG(IEnumerable<T> collection) : this() {
            if (collection == null)
                return;

            foreach (T element in collection)
                this[element] = 1f;
        }

        /// <summary>
        /// Initializes a new instance of WeightedRNG using the weights and elements specifed by a dictionary.
        /// </summary>
        /// <remarks>
        /// If <paramref name="dictionary"/> is null, the resultant WeightedRNG instance will be empty.
        /// </remarks>
        /// <param name="dictionary">a map between. </param>
        public WeightedRNG(IDictionary<T, float> dictionary) : this() {
            if (dictionary == null)
                return;

            foreach (KeyValuePair<T, float> element in dictionary)
                this[element.Key] = element.Value;
        }

        /// <summary>
        /// Indexer to get or set the weight of an element.
        /// When using the setter, if the WeigthedRNG does not contain the element, it will be added.
        /// </summary>
        /// <param name="index">the element to edit/retrieve the weight of</param>
        /// <returns>the weight of the element</returns>
        public float this[T index] {
            get { return _weights.ContainsKey(index) ? _weights[index] : 0f; }
            set {
                if (Contains(index))
                    _weightSum -= _weights[index];
                _weights[index] = value;
                _weightSum += value;
            }
        }

        /// <summary>
        /// Returns a randomly selected element from the WeightedRNG.
        /// </summary>
        /// <remarks>
        /// The probability that each element is selected is based on the relative weights of each element.
        /// Those with higher weights have porportionally higher probability of being selected.
        /// This operation runs in worst case O(n) time, where n is the Count of elements in the WeightedRNG.
        /// </remarks>
        /// <exception cref="InvalidOperationException">thrown if the WeightedRNG is empty</exception>
        /// <returns>a randomly selected</returns>
        public T Select() {
            if (Count <= 0)
                throw new InvalidOperationException();
            UnityEngine.Debug.Log(_weightSum);
            float randomValue = Random.value * _weightSum;
            UnityEngine.Debug.Log(randomValue);
            foreach (KeyValuePair<T, float> element in _weights) {
                randomValue -= element.Value;
                UnityEngine.Debug.Log(randomValue);
                if (randomValue <= 0)
                    return element.Key;
            }
            return default(T);
        }

#region ICollection Implemenation
        public void CopyTo(T[] array, int arrayIndex) {
            _weights.Keys.CopyTo(array, arrayIndex);
        }

        public bool Remove(T obj) {
            if (!_weights.ContainsKey(obj))
                return false;

            bool success = _weights.Remove(obj);
            if(success)
                _weightSum -= _weights[obj];
            return success;
        }

        /// <summary>
        /// A count of how many elements are in the WeightedRNG.
        /// </summary>
        public int Count { get { return _weights.Count; } }

        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Adds an item to the WeightedRNG with a weight of 1.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item) {
            this[item] = 1;
        }

        /// <summary>
        /// Clears all elements from the WeightedRNG
        /// </summary>
        public void Clear() {
            _weights.Clear();
            _weightSum = 0;
        }

        /// <summary>
        /// Checks if the WeightedRNG contains a certain element.
        /// </summary>
        /// <param name="obj">the element to check for</param>
        /// <returns>whether the element is </returns>
        public bool Contains(T obj) {
            return _weights.ContainsKey(obj);
        }

        public IEnumerator<T> GetEnumerator() {
            return _weights.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
#endregion
    }

}