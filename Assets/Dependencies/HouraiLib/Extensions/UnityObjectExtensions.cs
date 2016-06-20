// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEngine;

namespace HouraiTeahouse {
    /// <summary> A set of extenion methods for all Unity Objects. </summary>
    public static class UnityObjectExtensions {
        /// <summary> Destroys objects if they exist. Shorthand for Object.Destroy() </summary>
        /// <param name="obj"> the object to destroy </param>
        public static void Destroy(this Object obj) {
            if (obj)
                Object.Destroy(obj);
        }

        /// <summary> Destroys objects if they exist after a specified time. Shorthand for Object.Destroy() </summary>
        /// <param name="obj"> the object to destroy </param>
        /// <param name="t"> the delay before destroying the object </param>
        public static void Destroy(this Object obj, float t) {
            if (obj)
                Object.Destroy(obj, t);
        }

        /// <summary> Creates a copy of the object given. </summary>
        /// <remarks> Returns null if <paramref name="obj" /> is null. </remarks>
        /// <typeparam name="T"> the type of the object to instantiate </typeparam>
        /// <param name="obj"> the original object </param>
        /// <returns> the copied object </returns>
        public static T Duplicate<T>(this T obj) where T : Object {
            if (!obj)
                return null;
            return Object.Instantiate(obj);
        }

        /// <summary> Instantiates a copy of the object given at a certain location. </summary>
        /// <remarks> The rotation of the object is untouched and is copied as-is from the source object. Does the same thing as
        /// Duplicate(obj) if obj is not a GameObject or a Component. Returns null if <paramref name="obj" /> is null. </remarks>
        /// <typeparam name="T"> the type of the Unity object to instantiate </typeparam>
        /// <param name="obj"> the source object to instantiate from </param>
        /// <param name="position"> the position to place it. </param>
        /// <returns> the copied instance </returns>
        public static T Duplicate<T>(this T obj, Vector3 position)
            where T : Object {
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

        /// <summary> Instantiates a copy of the object given with a certain rotation. </summary>
        /// <remarks> The position of the object is untouched and is copied as-is fromthe source object. Does the same thing as
        /// Duplicate(obj) if obj is not a GameObject or a Component. Returns null if <paramref name="obj" /> is null. </remarks>
        /// <typeparam name="T"> the type of the Unity object to instantiate </typeparam>
        /// <param name="obj"> the source object to instantiate from </param>
        /// <param name="rotation"> the rotation to use on the object </param>
        /// <returns> the copied instance </returns>
        public static T Duplicate<T>(this T obj, Quaternion rotation)
            where T : Object {
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

        /// <summary> Instantiates a copy of the object given with a certain rotation. </summary>
        /// <remarks> The position of the object is untouched and is copied as-is fromthe source object. Does the same thing as
        /// Duplicate(obj) if obj is not a GameObject or a Component. This applies a 2D rotation only (only rotates along the Z
        /// axis). Returns null if <paramref name="obj" /> is null. </remarks>
        /// <typeparam name="T"> the type of the Unity object to instantiate </typeparam>
        /// <param name="obj"> the source object to instantiate from </param>
        /// <param name="rotation"> the rotation to use on the object </param>
        /// <returns> the copied instance </returns>
        public static T Duplicate<T>(this T obj, float rotation)
            where T : Object {
            return obj.Duplicate(Quaternion.Euler(0f, 0f, rotation));
        }

        /// <summary> Creates a copy of an object at a specified positiona nd rotation. </summary>
        /// <remarks> Returns null if <paramref name="obj" /> is null. </remarks>
        /// Does the same thing as Duplicate(obj) if obj is not a GameObject or a Component.
        /// This applies a 2D rotation only (only rotates along the Z axis)
        /// <typeparam name="T"> the type of the object to instantiates </typeparam>
        /// <param name="obj"> the original object </param>
        /// <param name="position"> the position to place </param>
        /// <param name="rotation"> the rotation to use on the object </param>
        /// <returns> the copied instance </returns>
        public static T Duplicate<T>(this T obj,
                                     Vector3 position,
                                     Quaternion rotation) where T : Object {
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

        /// <summary> Creates a copy of an object at a specified positiona nd rotation. </summary>
        /// <remarks> Does the same thing as Duplicate(obj) if obj is not a GameObject or a Component. This applies a 2D rotation
        /// only (only rotates along the Z axis)> Returns null if <paramref name="obj" /> is null. </remarks>
        /// <typeparam name="T"> the type of the object to instantiates </typeparam>
        /// <param name="obj"> the original object </param>
        /// <param name="position"> the position to place </param>
        /// <param name="rotation"> the rotation to use on the object </param>
        /// <returns> the copied instance </returns>
        public static T Duplicate<T>(this T obj,
                                     Vector3 position,
                                     float rotation) where T : Object {
            return obj.Duplicate(position, Quaternion.Euler(0f, 0f, rotation));
        }

        /// <summary> Gets an object's GameObject, if it has one. Returns itself if it is a GameObject. Returns the containing
        /// GameObject if it is a component. Returns null otherwise. </summary>
        /// <param name="obj"> the object in question </param>
        /// <returns> The object GameObject, null if not associated with one </returns>
        public static GameObject GetGameObject(this Object obj) {
            var go = obj as GameObject;
            var comp = obj as Component;
            if (go != null)
                return go;
            if (comp != null)
                return comp.gameObject;
            return null;
        }
    }
}