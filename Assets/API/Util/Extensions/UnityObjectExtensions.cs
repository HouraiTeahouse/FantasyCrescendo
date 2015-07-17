using UnityEngine;
using System.Collections.Generic;

namespace Genso.API {

    public static class UnityObjectExtensions
    {

        /// <summary>
        /// Instantiates a copy of the Unity object.
        /// Shorthand for Object.Copy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetObject"></param>
        /// <returns></returns>
        public static T Copy<T>(this T targetObject) where T : Object
        {
            if (targetObject == null)
                return null;
            return Object.Instantiate(targetObject);
        }

        public static T Copy<T>(this T targetObject, Vector3 position) where T : Object
        {
            if (targetObject == null)
                return null;
            return Object.Instantiate(targetObject, position, Quaternion.identity) as T;
        }

        public static T Copy<T>(this T targetObject, Vector3 position, Quaternion rotation) where T : Object
        {
            if (targetObject == null)
                return null;
            return Object.Instantiate(targetObject, position, rotation) as T;
        }

        /// <summary>
        /// Destroys the Unity object.
        /// Shorthand for Object.Destroy.
        /// If targetObject has already been destroyed, this function does nothing.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="allowDestroyingAssets"></param>
        public static void Destroy<T>(this T targetObject, bool allowDestroyingAssets = false) where T : Object
        {
            if (targetObject == null)
                return;

            if (Application.isPlaying)
            {
                var gameObject = targetObject as GameObject;
                if (gameObject != null)
                {
                    gameObject.transform.parent = null;
                }
                Object.Destroy(targetObject);
            }
            else
            {
                Object.DestroyImmediate(targetObject, allowDestroyingAssets);
            }
        }

        /// <summary>
        /// Destroys all of the Unity objects in a collection.
        /// Does nothingi f the collection is null.
        /// </summary>
        /// <param name="targetObjects"></param>
        /// <param name="allowDestroyingAssets"></param>
        public static void DestroyAll<T>(this IEnumerable<T> targetObjects, bool allowDestroyingAssets = false) where T : Object
        {
            if (targetObjects == null)
                return;

            Object[] arrayTest = targetObjects as Object[];
            if (arrayTest != null)
                for (var i = 0; i < arrayTest.Length; i++)
                    arrayTest[i].Destroy();
            else
                foreach (Object target in targetObjects)
                    target.Destroy();
        }

    }


}