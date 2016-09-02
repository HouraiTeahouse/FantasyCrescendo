using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse {

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExtensionAttribute : Attribute {

        public ExtensionAttribute(Type type, bool required = false, bool disallowMultiple = false) {
            TargetType = type;
            Required = required;
            DisallowMultiple = disallowMultiple;
        }

        public Type TargetType { get; set; }
        public bool Required { get; set; }
        public bool DisallowMultiple { get; set; }

    }

    // A base class for extendable scriptable objects
    // Allows creation of general scriptable objects that can be extended later in particular contexts.
    public abstract class ExtendableObject : ScriptableObject {

        [SerializeField]
        [HideInInspector]
        List<ScriptableObject> _extensions;

        ReadOnlyCollection<ScriptableObject> _readOnlyExtensions;

        public ReadOnlyCollection<ScriptableObject> Extensions {
            get {
                if (_readOnlyExtensions == null)
                    _readOnlyExtensions = new ReadOnlyCollection<ScriptableObject>(_extensions);
                return _readOnlyExtensions;
            }
        }

        // Gets the first instance of the given 
        public ScriptableObject GetExtension(Type type) {
            return _extensions.Find(Check.NotNull(type).IsInstanceOfType);
        }

        // Generic overload for GetExtension
        public T GetExtension<T>() { return GetExtensions<T>().FirstOrDefault(); }

        // Gets all instances of a given type
        public List<ScriptableObject> GetExtensions(Type type) {
            return _extensions.FindAll(Check.NotNull(type).IsInstanceOfType);
        }

        // Generic overload for GetExtensions
        public IEnumerable<T> GetExtensions<T>() { return _extensions.OfType<T>(); }

        // Checks whether an extension of a given type is added.
        public bool HasExtension(Type type) { return GetExtension(type) != null; }

        // Generic overload for HasExtetnsion
        public bool HasExtension<T>() { return GetExtensions<T>().Any(); }

        // Adds an extension of a given type to the object.
        public ScriptableObject AddExtension(Type type) {
            ScriptableObject extension = CreateInstance(type);
            extension.hideFlags = HideFlags.HideInHierarchy;
#if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode) {
                AssetDatabase.AddObjectToAsset(extension, this);
                AssetDatabase.SaveAssets();
            }
#endif
            _extensions.Add(extension);
            extension.hideFlags = HideFlags.HideInHierarchy;
            return extension;
        }

        // Generic overload for AddExtension
        public T AddExtension<T>() where T : ScriptableObject {
            var extension = CreateInstance<T>();
            extension.hideFlags = HideFlags.HideInHierarchy;
#if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode) {
                AssetDatabase.AddObjectToAsset(extension, this);
                AssetDatabase.SaveAssets();
            }
#endif
            _extensions.Add(extension);
            return extension;
        }

        // Removes an extension from this current object.
        public bool RemoveExtension(ScriptableObject extension) {
            if (!_extensions.Remove(extension))
                return false;
#if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode) {
                DestroyImmediate(extension, true);
                AssetDatabase.SaveAssets();
            }
#endif
            return true;
        }

    }

}
