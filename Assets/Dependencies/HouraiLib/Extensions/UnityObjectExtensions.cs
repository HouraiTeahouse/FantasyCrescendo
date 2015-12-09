using UnityEngine;
using System.Collections;

namespace Hourai {

    public static class UnityObjectExtensions {

        /// <summary>
        /// Destroys objects if they exist.
        /// Shorthand for Object.Destroy()
        /// </summary>
        /// <param name="obj">the object to destroy</param>
        public static void Destroy(this Object obj) {
            if (obj)
                Object.Destroy(obj);
        }

        /// <summary>
        /// Destroys objects if they exist after a specified time.
        /// Shorthand for Object.Destroy()
        /// </summary>
        /// <param name="obj">the object to destroy</param>
        /// <param name="t">the delay before destroying the object</param>
        public static void Destroy(this Object obj, float t) {
            if (obj)
                Object.Destroy(obj, t);
        }

        /// <summary>
        /// Creates a copy of the object given.
        /// Returns null if <paramref name="obj"/> is null.
        /// </summary>
        /// <typeparam name="T">the type of the object to instantiate</typeparam>
        /// <param name="obj">the original object</param>
        /// <returns>the copied object</returns>
        public static T Duplicate<T>(this T obj) where T : Object {
            return !obj ? null : Object.Instantiate(obj);
        }

        public static T Duplicate<T>(this T obj, Vector3 position) where T : Object {
            T copy = obj.Duplicate();
            if (!copy)
                return copy;
            Transform transform = null;
            var comp = copy as Component;
            var go = copy as GameObject;
            if (comp)
                transform = comp.transform;
            if (go)
                transform = go.transform;
            if (transform)
                transform.position = position;
            return copy;
        }

        public static T Duplicate<T>(this T obj, Quaternion rotation) where T : Object {
            T copy = obj.Duplicate();
            if (!copy)
                return copy;
            Transform transform = null;
            var comp = copy as Component;
            var go = copy as GameObject;
            if (comp)
                transform = comp.transform;
            if (go)
                transform = go.transform;
            if (transform)
                transform.rotation = rotation;
            return copy;
        }

        public static T Duplicate<T>(this T obj, float rotation) where T : Object {
            return obj.Duplicate(Quaternion.Euler(0f, 0f, rotation));
        }

        /// <summary>
        /// Creates a copy of an object at a specified positiona nd rotation.
        /// Returns null if <paramref name="obj"/> is null.
        /// </summary>
        /// <typeparam name="T">the type of the object to instantiates</typeparam>
        /// <param name="obj">the original object</param>
        /// <param name="position">the position to place </param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static T Duplicate<T>(this T obj, Vector3 position, Quaternion rotation) where T : Object {
            T copy = obj.Duplicate();
            if (!copy)
                return copy;
            Transform transform = null;
            var comp = copy as Component;
            var go = copy as GameObject;
            if (comp)
                transform = comp.transform;
            if (go)
                transform = go.transform;
            if (!transform)
                return copy;
            transform.position = position;
            transform.rotation = rotation;
            return copy;
        }

        public static T Duplicate<T>(this T obj, Vector3 position, float rotation) where T : Object {
            return obj.Duplicate(position, Quaternion.Euler(0f, 0f, rotation));
        }

    }

}

