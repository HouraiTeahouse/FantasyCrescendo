using UnityEngine;
using System.Collections.Generic;

public static class UnityObjectExtensions {

    /// <summary>
    /// Instantiates a copy of the Unity object.
    /// Shorthand for Object.Instantiate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="targetObject"></param>
    /// <returns></returns>
    public static T Instantiate<T>(this T targetObject) where T : Object {
        return Object.Instantiate(targetObject);
    }

    public static T Instantiate<T>(this T targetObject, Vector3 position) where T : Object {
        return Object.Instantiate(targetObject, position, Quaternion.identity) as T;
    }

    public static T Instantiate<T>(this T targetObject, Vector3 position, Quaternion rotation) where T : Object {
        return Object.Instantiate(targetObject, position, rotation) as T;
    }

    /// <summary>
    /// Destroys the Unity object.
    /// Shorthand for Object.Destroy.
    /// If targetObject has already been destroyed, this function does nothing.
    /// </summary>
    /// <param name="targetObject"></param>
    /// <param name="allowDestroyingAssets"></param>
    public static void Destroy(this Object targetObject, bool allowDestroyingAssets = false) {
        if (targetObject == null)
            return;

        if (Application.isPlaying) {
            if (targetObject is GameObject) {
                ((GameObject) targetObject).transform.parent = null;
            }
            Object.Destroy(targetObject);
        } else {
            Object.DestroyImmediate(targetObject, allowDestroyingAssets);
        }
    }

    /// <summary>
    /// Destroys all of the Unity objects in a collection.
    /// Does nothingi f the collection is null.
    /// </summary>
    /// <param name="targetObjects"></param>
    /// <param name="allowDestroyingAssets"></param>
    public static void DestroyAll(this IEnumerable<Object> targetObjects, bool allowDestroyingAssets = false) {
        if (targetObjects == null)
            return;

        Object[] arrayTest = targetObjects as Object[];
        if(arrayTest != null)
            for(var i = 0; i < arrayTest.Length; i++)
                arrayTest[i].Destroy();
        else
            foreach(Object target in targetObjects)
                target.Destroy();
    }

}
