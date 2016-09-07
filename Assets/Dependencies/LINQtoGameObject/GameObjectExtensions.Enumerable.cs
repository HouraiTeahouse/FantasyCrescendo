using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Linq {

    public static partial class GameObjectExtensions {
#if UNITY_EDITOR
        class ComponentCache<T> {

            public static readonly List<T> Instance = new List<T>(); // for no allocate on UNITY_EDITOR

        }
#endif

        /// <summary> Returns a collection of GameObjects that contains the ancestors of every GameObject in the source collection. </summary>
        public static IEnumerable<GameObject> Ancestors(this IEnumerable<GameObject> source) {
            foreach (GameObject item in source) {
                AncestorsEnumerable.Enumerator e = item.Ancestors().GetEnumerator();
                while (e.MoveNext()) {
                    yield return e.Current;
                }
            }
        }

        /// <summary> Returns a collection of GameObjects that contains every GameObject in the source collection, and the
        /// ancestors of every GameObject in the source collection. </summary>
        public static IEnumerable<GameObject> AncestorsAndSelf(this IEnumerable<GameObject> source) {
            foreach (GameObject item in source) {
                AncestorsEnumerable.Enumerator e = item.AncestorsAndSelf().GetEnumerator();
                while (e.MoveNext()) {
                    yield return e.Current;
                }
            }
        }

        /// <summary> Returns a collection of GameObjects that contains the descendant GameObjects of every GameObject in the
        /// source collection. </summary>
        public static IEnumerable<GameObject> Descendants(this IEnumerable<GameObject> source) {
            foreach (GameObject item in source) {
                DescendantsEnumerable.Enumerator e = item.Descendants().GetEnumerator();
                while (e.MoveNext()) {
                    yield return e.Current;
                }
            }
        }

        /// <summary> Returns a collection of GameObjects that contains every GameObject in the source collection, and the
        /// descendent GameObjects of every GameObject in the source collection. </summary>
        public static IEnumerable<GameObject> DescendantsAndSelf(this IEnumerable<GameObject> source) {
            foreach (GameObject item in source) {
                DescendantsEnumerable.Enumerator e = item.DescendantsAndSelf().GetEnumerator();
                while (e.MoveNext()) {
                    yield return e.Current;
                }
            }
        }

        /// <summary> Returns a collection of the child GameObjects of every GameObject in the source collection. </summary>
        public static IEnumerable<GameObject> Children(this IEnumerable<GameObject> source) {
            foreach (GameObject item in source) {
                ChildrenEnumerable.Enumerator e = item.Children().GetEnumerator();
                while (e.MoveNext()) {
                    yield return e.Current;
                }
            }
        }

        /// <summary> Returns a collection of GameObjects that contains every GameObject in the source collection, and the child
        /// GameObjects of every GameObject in the source collection. </summary>
        public static IEnumerable<GameObject> ChildrenAndSelf(this IEnumerable<GameObject> source) {
            foreach (GameObject item in source) {
                ChildrenEnumerable.Enumerator e = item.ChildrenAndSelf().GetEnumerator();
                while (e.MoveNext()) {
                    yield return e.Current;
                }
            }
        }

        /// <summary> Destroy every GameObject in the source collection safety(check null). </summary>
        /// <param name="useDestroyImmediate"> If in EditMode, should be true or pass !Application.isPlaying. </param>
        public static void Destroy(this IEnumerable<GameObject> source, bool useDestroyImmediate = false) {
            foreach (GameObject item in source) {
                item.Destroy(useDestroyImmediate, false); // doesn't detach.
            }
        }

        /// <summary> Returns a collection of specified component in the source collection. </summary>
        public static IEnumerable<T> OfComponent<T>(this IEnumerable<GameObject> source) where T : Component {
            foreach (GameObject item in source) {
#if UNITY_EDITOR
                List<T> cache = ComponentCache<T>.Instance;
                item.GetComponents<T>(cache);
                if (cache.Count != 0) {
                    T component = cache[0];
                    cache.Clear();
                    yield return component;
                }
#else
                        
                var component = item.GetComponent<T>();
                if (component != null)
                {
                    yield return component;
                }
#endif
            }
        }

        /// <summary> Store element into the buffer, return number is size. array is automaticaly expanded. </summary>
        public static int ToArrayNonAlloc<T>(this IEnumerable<T> source, ref T[] array) {
            var index = 0;
            foreach (T item in source) {
                if (array.Length == index) {
                    int newSize = index == 0 ? 4 : index * 2;
                    Array.Resize(ref array, newSize);
                }
                array[index++] = item;
            }

            return index;
        }

    }

}
