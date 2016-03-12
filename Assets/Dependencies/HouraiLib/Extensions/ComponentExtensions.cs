using System;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse {
    /// <summary>
    /// A set of extention methods for all components.
    /// </summary>
    public static class ComponentExtensions {
        /// <summary>
        /// Gets the GameObjects of all
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="components"></param>
        /// <returns></returns>
        public static IEnumerable<GameObject> GetGameObject<T>(this IEnumerable<T> components) where T : Component {
            if (components == null)
                throw new ArgumentNullException("components");
            foreach (T component in components)
                if (component != null)
                    yield return component.gameObject;
        }

        /// <summary>
        /// Gets a component of a certain type.
        /// If one doesn't exist, one will be added and returned.
        /// </summary>
        /// <exception cref="ArgumentNullException">thrown if <paramref name="component"/> is null</exception>
        /// <typeparam name="T">the type of the component to retrieve</typeparam>
        /// <param name="component">the Component attached to the GameObject to retrieve the Component</param>
        /// <returns>the retrieved Component</returns>
        public static T GetOrAddComponent<T>(this Component component) where T : Component {
            if (!component)
                throw new ArgumentNullException();
            GameObject gameObject = component.gameObject;
            var attempt = gameObject.GetComponent<T>();
            return attempt ? attempt : gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Gets a component of a certain type on a GameObject.
        /// Works exactly like the normal GetComponent, but also logs an error in the console if one is not found.
        /// </summary>
        /// <exception cref="ArgumentNullException">thrown if <paramref name="component"/> is null</exception>
        /// <typeparam name="T">the type of the component to retrieve</typeparam>
        /// <param name="component">the GameObject to retrieve the Component</param>
        /// <returns>the retrieved Component</returns>
        public static T SafeGetComponent<T>(this Component component) where T : class {
            if (!component)
                throw new ArgumentNullException();
            GameObject gameObject = component.gameObject;
            var attempt = gameObject.GetComponent<T>();
            if (attempt != null)
                Debug.LogWarning("Attempted to find a component of type " + typeof (T) + ", but did not find one.",
                    gameObject);
            return attempt;
        }
    }
}