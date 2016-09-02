using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> A set of extention methods for all components. </summary>
    public static class ComponentExtensions {

        /// <summary> Gets the GameObjects of all </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="components"> </param>
        /// <returns> </returns>
        public static IEnumerable<GameObject> GetGameObject<T>(this IEnumerable<T> components) where T : Component {
            return from component in components.IgnoreNulls() select component.gameObject;
        }

        /// <summary> Gets a component of a certain type. If one doesn't exist, one will be added and returned. </summary>
        /// <exception cref="ArgumentNullException"> <paramref name="component" /> is null </exception>
        /// <typeparam name="T"> the type of the component to retrieve </typeparam>
        /// <param name="component"> the Component attached to the GameObject to retrieve the Component </param>
        /// <returns> the retrieved Component </returns>
        public static T GetOrAddComponent<T>(this Component component) where T : Component {
            GameObject gameObject = Check.NotNull(component).gameObject;
            var attempt = gameObject.GetComponent<T>();
            return attempt ? attempt : gameObject.AddComponent<T>();
        }

        /// <summary> Gets a component of a certain type on a GameObject. Works exactly like the normal GetComponent, but also logs
        /// an error in the console if one is not found. </summary>
        /// <exception cref="ArgumentNullException"> <paramref name="component" /> is null </exception>
        /// <typeparam name="T"> the type of the component to retrieve </typeparam>
        /// <param name="component"> the GameObject to retrieve the Component </param>
        /// <returns> the retrieved Component </returns>
        public static T SafeGetComponent<T>(this Component component) where T : class {
            GameObject gameObject = Check.NotNull(component).gameObject;
            var attempt = gameObject.GetComponent<T>();
            if (attempt != null)
                Log.Warning("Attempted to find a component of type {0}, but did not find one.", typeof(T));
            return attempt;
        }

    }

}