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
        public static IEnumerable<GameObject> GetGameObject<T>(
            this IEnumerable<T> components) where T : Component {
            Check.NotNull("components", components);
            return from component in components.IgnoreNulls()
                   select component.gameObject;
        }

        /// <summary> Gets a component of a certain type. If one doesn't exist, one will be added and returned. </summary>
        /// <exception cref="ArgumentNullException"> <paramref name="component" /> is null </exception>
        /// <typeparam name="T"> the type of the component to retrieve </typeparam>
        /// <param name="component"> the Component attached to the GameObject to retrieve the Component </param>
        /// <returns> the retrieved Component </returns>
        public static T GetOrAddComponent<T>(this Component component)
            where T : Component {
            Check.NotNull("component", component);
            GameObject gameObject = component.gameObject;
            var attempt = gameObject.GetComponent<T>();
            return attempt ? attempt : gameObject.AddComponent<T>();
        }

        /// <summary> Gets a component of a certain type on a GameObject. Works exactly like the normal GetComponent, but also logs
        /// an error in the console if one is not found. </summary>
        /// <exception cref="ArgumentNullException"> <paramref name="component" /> is null </exception>
        /// <typeparam name="T"> the type of the component to retrieve </typeparam>
        /// <param name="component"> the GameObject to retrieve the Component </param>
        /// <returns> the retrieved Component </returns>
        public static T SafeGetComponent<T>(this Component component)
            where T : class {
            Check.NotNull("component", component);
            GameObject gameObject = component.gameObject;
            var attempt = gameObject.GetComponent<T>();
            if (attempt != null)
                Log.Warning(
                    "Attempted to find a component of type {0}, but did not find one.",
                    typeof(T));
            return attempt;
        }
    }
}