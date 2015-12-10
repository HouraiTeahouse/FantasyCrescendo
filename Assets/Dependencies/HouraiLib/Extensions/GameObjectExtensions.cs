using System;
using UnityEngine;

namespace Hourai {
    
    public static class GameObjectExtensions {

        /// <summary>
        /// Gets a component of a certain type.
        /// If one doesn't exist, one will be added and returned.
        /// </summary>
        /// <exception cref="ArgumentNullException">thrown if <paramref name="gameObject"/> is null</exception>
        /// <typeparam name="T">the type of the component to retrieve</typeparam>
        /// <param name="gameObject">the GameObject to retrieve the Component</param>
        /// <returns>the retrieved Component</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component {
            if (!gameObject)
                throw new ArgumentNullException();
            var attempt = gameObject.GetComponent<T>();
            return attempt ? attempt : gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Gets a component of a certain type on a GameObject.
        /// Works exactly like the normal GetComponent, but also logs an error in the console if one is not found.
        /// </summary>
        /// <exception cref="ArgumentNullException">thrown if <paramref name="gameObject"/> is null</exception>
        /// <typeparam name="T">the type of the component to retrieve</typeparam>
        /// <param name="gameObject">the GameObject to retrieve the Component</param>
        /// <returns>the retrieved Component</returns>
        public static T SafeGetComponent<T>(this GameObject gameObject) where T : class {
            if (!gameObject)
                throw new ArgumentNullException();
            var attempt = gameObject.GetComponent<T>();
            if(attempt != null)
                Debug.LogWarning("Attempted to find a component of type " + typeof(T) + ", but did not find one.", gameObject);
            return attempt;
        }

    }

}

