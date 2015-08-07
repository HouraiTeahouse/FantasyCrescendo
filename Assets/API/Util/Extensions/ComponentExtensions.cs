using System;
using System.Collections.Generic;
using UnityEngine;

namespace Crescendo.API {

    public static class ComponentExtensions {

        public static bool CompareTags(this Component component, IEnumerable<string> tags) {
            return component.gameObject.CompareTags(tags);
        }

        public static T SafeGetComponent<T>(this Component component) where T : class {
            return component.gameObject.SafeGetComponent<T>();
        }

        public static bool CheckLayer(this Component component, int mask) {
            return component.gameObject.CheckLayer(mask);
        }

        public static T GetOrAddComponent<T>(this Component component) where T : Component {
            return component.gameObject.GetOrAddComponent<T>();
        }

        public static Component GetOrAddComponent(this Component component, Type componentType) {
            return component.gameObject.GetOrAddComponent(componentType);
        }

        public static T GetIComponent<T>(this Component component) where T : class {
            return component.gameObject.GetIComponent<T>();
        }

        public static T[] GetIComponents<T>(this Component component) where T : class {
            return component.gameObject.GetIComponents<T>();
        }

        public static T GetIComponentInChildren<T>(this Component component) where T : class {
            return component.gameObject.GetIComponentInChildren<T>();
        }

        public static T[] GetIComponentsInChildren<T>(this Component component) where T : class {
            return component.gameObject.GetIComponentsInChildren<T>();
        }

        public static T GetIComponentInParent<T>(this Component component) where T : class {
            return component.gameObject.GetIComponentInParent<T>();
        }

        public static T[] GetIComponentsInParent<T>(this Component component) where T : class {
            return component.gameObject.GetIComponentsInParent<T>();
        }

    }

}