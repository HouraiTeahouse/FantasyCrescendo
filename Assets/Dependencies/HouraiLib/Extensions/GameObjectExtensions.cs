using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse {
    
    /// <summary>
    /// Set of extension methods for GameObjects
    /// </summary>
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
                throw new ArgumentNullException("gameObject");
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
                throw new ArgumentNullException("gameObject");
            var attempt = gameObject.GetComponent<T>();
            if(attempt != null)
                Debug.LogWarning("Attempted to find a component of type " + typeof(T) + ", but did not find one.", gameObject);
            return attempt;
        }

        /// <summary>
        /// Gets all Components of a certain type that are attached to a set of GameObjects.
        /// </summary>
        /// <typeparam name="T">the type of component to retrieve, can be an interface</typeparam>
        /// <param name="gameObjects">the GameObjects to retrieve</param>
        /// <returns>an enumeration of all components of the type attached to the GameObjects</returns>
        public static IEnumerable<T> GetComponents<T>(this IEnumerable<GameObject> gameObjects) where T : class {
            if(gameObjects == null)
                throw new ArgumentNullException("gameObjects");
            foreach(GameObject gameObject in gameObjects)
                if(gameObject != null)
                    foreach (T component in gameObject.GetComponents<T>())
                        yield return component;
        } 

    }

}

