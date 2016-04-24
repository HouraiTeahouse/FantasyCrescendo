using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HouraiTeahouse {
    /// <summary>
    /// A PropertyAttribute for the Unity Editor. Marks a string field to store a path to an asset stored in a Resources folder.
    /// The resultant string can be used with Resources.Load to get said asset.
    /// The Unity Editor UI shows a object field instead of a string field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    [HelpURL("http://wiki.houraiteahouse.net/index.php/Dev:Resources#ResourcePathAttribute_and_Resource_Wrapper")]
    public class ResourceAttribute : PropertyAttribute {
        /// <summary>
        /// The type of asset to be stored. All instances of this type, including those of derived types, can be used.
        /// </summary>
        public Type TypeRestriction { get; private set; }

        /// <summary>
        /// Initializes a new instance of ResourceAttribute.
        /// </summary>
        /// <param name="type">Optional type restriction on the type of Resource object to use. No restriction is applied if null or not derived from UnityEngine.Object</param>
        public ResourceAttribute(Type type = null) {
            TypeRestriction = typeof (Object);
            if (type == null)
                return;
            if (typeof (Object).IsAssignableFrom(type))
                TypeRestriction = type;
            else
                Log.Warning(
                    "Trying to get a resource type restriction on type: {0} is impossible. Use a type derived from UnityEngine.Object.",
                    type.FullName);
        }
    }
}
