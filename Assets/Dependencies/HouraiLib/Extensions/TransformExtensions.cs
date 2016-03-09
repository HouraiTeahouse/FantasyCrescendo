using System;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse {

    public static class TransformExtensions {

        public static void SetX(this Transform transform, float x, bool local = false) {
            transform.SetPositionLocation(0, x, local);
        }

        public static void SetY(this Transform transform, float y, bool local = false) {
            transform.SetPositionLocation(1, y, local);
        }

        public static void SetZ(this Transform transform, float z, bool local = false) {
            transform.SetPositionLocation(2, z, local);
        }

        /// <summary>
        /// Copys the position and rotation of another transform onto one.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="target"></param>
        /// <exception cref="ArgumentNullException">thrown if <paramref name="transform"/>
        ///  or <paramref name="target"/> are null</exception>
        public static void Copy(this Transform transform, Transform target) {
            if(!transform || !target)
                throw new ArgumentNullException();
            transform.position = target.position;
            transform.rotation = target.rotation;
        }

        /// <summary>
        /// Finds the lowest common ancestor between two Transforms.
        /// Returns null if either are null or both are not part of the same Transform hiearchy.
        /// </summary>
        /// <param name="transform">the first transform</param>
        /// <param name="other">the second transform</param>
        /// <returns>the lowest common ancestor between the two transforms</returns>
        public static Transform FindCommonAncestor(this Transform transform, Transform other) {
            if (!transform || !other || transform.root != other.root)
                return null;
            var s1 = new HashSet<Transform>();
            var s2 = new HashSet<Transform>();
            var t1 = transform;
            var t2 = other;
            while (t1 || t2) {
                if (t1) {
                    if (s2.Contains(t1))
                        return t1;
                    s1.Add(t1);
                    t1 = t1.parent;
                }
                if (t2) {
                    if (s1.Contains(t2))
                        return t2;
                    s2.Add(t2);
                    t2 = t2.parent;
                }
            }
            return null;
        }

        static void SetPositionLocation(this Transform transform, int component, float value, bool local) {
            if (!transform)
                throw new ArgumentNullException();
            Vector3 position = local ? transform.localPosition : transform.position;
            position[component] = value;
            if (local)
                transform.localPosition = position;
            else
                transform.position = position;
        }

    }

}
