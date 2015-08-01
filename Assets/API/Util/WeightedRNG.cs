using UnityEngine;
using System.Collections.Generic;

namespace Crescendo.API
{

    public class WeightedRNG<T>
    {

        private float weightSum;
        private Dictionary<T, float> weights;

        public WeightedRNG()
        {
            weights = new Dictionary<T, float>();
        }

        /// <summary>
        /// Creates a WeightedRNG instance from a given collection.
        /// Every element has the same weight of 1.
        /// </summary>
        /// <param name="collection"></param>
        public WeightedRNG(IEnumerable<T> collection) : this()
        {
            if (collection == null)
                return;

            foreach (var element in collection)
                this[element] = 1f;
        }
        
        public WeightedRNG(IDictionary<T, float> dictionary) : this()
        {
            if (dictionary == null)
                return;

            foreach (var element in dictionary)
                this[element.Key] = element.Value;
        }

        public T Select()
        {
            if (weights.Count <= 0)
                return default(T);

            float randomValue = Random.value * weightSum;
            foreach(var element in weights)
            {
                randomValue -= element.Value;
                if (randomValue <= 0)
                    return element.Key;
            }
            return default(T);
        }

        public void Add(T obj, float weight)
        {
            this[obj] = weight;
        }

        public void Remove(T obj)
        {
            if (!weights.ContainsKey(obj))
                return;

            weightSum -= weights[obj];
            weights.Remove(obj);
        }

        public bool Contains(T obj)
        {
            return weights.ContainsKey(obj);
        }

        public float this[T index]
        {
            get { return weights.ContainsKey(index) ? weights[index] : 0f; }
            set
            {
                if(weights.ContainsKey(index))
                {
                    weightSum -= weights[index];
                    weightSum += value;
                }
                weights[index] = value;
            }
        }
    }

}