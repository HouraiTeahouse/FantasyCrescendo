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

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse {
    /// <summary> Set of extension methods for GameObjects </summary>
    public static class GameObjectExtensions {
        /// <summary> Gets a component of a certain type. If one doesn't exist, one will be added and returned. </summary>
        /// <typeparam name="T"> the type of the component to retrieve </typeparam>
        /// <param name="gameObject"> the GameObject to retrieve the Component </param>
        /// <returns> the retrieved Component </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="gameObject" /> is null </exception>
        public static T GetOrAddComponent<T>(this GameObject gameObject)
            where T : Component {
            Check.NotNull(gameObject);
            var attempt = gameObject.GetComponent<T>();
            return attempt ? attempt : gameObject.AddComponent<T>();
        }

        /// <summary> Gets a component of a certain type on a GameObject. Works exactly like the normal GetComponent, but also logs
        /// an error in the console if one is not found. </summary>
        /// <typeparam name="T"> the type of the component to retrieve </typeparam>
        /// <param name="gameObject"> the GameObject to retrieve the Component </param>
        /// <returns> the retrieved Component </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="gameObject" /> is null </exception>
        public static T SafeGetComponent<T>(this GameObject gameObject)
            where T : class {
            Check.NotNull(gameObject);
            var attempt = gameObject.GetComponent<T>();
            if (attempt != null)
                Log.Warning(
                    "Attempted to find a component of type {0}, but did not find one.",
                    typeof(T));
            return attempt;
        }

        /// <summary> Gets all Components of a certain type that are attached to a set of GameObjects. </summary>
        /// <typeparam name="T"> the type of component to retrieve, can be an interface </typeparam>
        /// <param name="gameObjects"> the GameObjects to retrieve </param>
        /// <returns> an enumeration of all components of the type attached to the GameObjects </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="gameObjects" /> is null </exception>
        public static IEnumerable<T> GetComponents<T>(
            this IEnumerable<GameObject> gameObjects) where T : class {
            return
                Check.NotNull(gameObjects)
                    .IgnoreNulls()
                    .SelectMany(gameObject => gameObject.GetComponents<T>());
        }

        /// <summary> Checks if a GameObject's layer is in a LayerMask or not. </summary>
        /// <param name="gameObject"> the GameObject to check </param>
        /// <param name="mask"> the layer to check </param>
        /// <returns> whether <paramref name="gameObject" /> fits the layer described </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="gameObject" /> is null </exception>
        public static bool LayerCheck(this GameObject gameObject, LayerMask mask) {
            return ((1 << Check.NotNull(gameObject).layer) & mask)
                != 0;
        }
    }
}
