using UnityEngine;
using System.Collections.Generic;

public static class UnityObjectExtensions {

    /// <summary>
    /// Destroys the Unity object.
    /// Shorthand for Object.Destroy.
    /// If targetObject has already been destroyed, this function does nothing.
    /// </summary>
    /// <param name="targetObject"></param>
    public static void Destroy(this Object targetObject) {
        if(targetObject)
            Object.Destroy(targetObject);
    }

    /// <summary>
    /// Destroys the Unity object immediately.
    /// Use only in Editor scripts. Use in actual games is not advised.
    /// Shorthand for Object.Destroy.
    /// If targetObject has already been destroyed, this function does nothing.
    /// </summary>
    /// <param name="targetObject"></param>
    public static void DestroyImmediate(this Object targetObject) {
        if(targetObject)
            Object.Destroy(targetObject);
    }

    /// <summary>
    /// Destroys all of the Unity objects in a collection.
    /// Does nothingi f the collection is null.
    /// </summary>
    /// <param name="targetObjects"></param>
    public static void DestroyAll(this IEnumerable<Object> targetObjects) {
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

    /// <summary>
    /// Destroys all of the Unity objects in a collection immediately.
    /// Use only in Editor scripts. Use in actual games is not advised.
    /// Does nothing if the collection is null.
    /// </summary>
    /// <param name="targetObjects"></param>
    public static void DestroyAllImmedate(this IEnumerable<Object> targetObjects) {
        if (targetObjects == null)
            return;

        Object[] arrayTest = targetObjects as Object[];
        if (arrayTest != null)
            for (var i = 0; i < arrayTest.Length; i++)
                arrayTest[i].DestroyImmediate();
        else
            foreach (Object target in targetObjects)
                target.DestroyImmediate();
    }

}
