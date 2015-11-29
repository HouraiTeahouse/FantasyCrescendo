using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hourai {

    [AttributeUsage(AttributeTargets.Field)]
    public class ResourceAttribute : PropertyAttribute {

        public Type TypeRestriction { get; private set; }

        public ResourceAttribute(Type type = null) {
            TypeRestriction = typeof (Object);
            if (type == null)
                return;
            if (typeof (UnityEngine.Object).IsAssignableFrom(type)) 
                TypeRestriction = type;
            else
                Debug.LogWarning("Trying to get a resource type restriction on type: " + type.FullName + " is impossible. Use a type derived from UnityEngine.Object.");
        }

    }

}