using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HouraiTeahouse {

    public static class TypeExtensions {

        /// <summary>
        /// Gets a custom attribute of a given type.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"><paramref name="provider"> is null.</exception>
        /// <param name="provider">The type of the attribute to get.</param>
        /// <returns>The fetched attribute, or null if none is found.</returns>
        public static T GetAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute {
            return GetAttributes<T>(provider).FirstOrDefault();
        }

        /// <summary>
        /// Gets all custom attributes of a given type.
        /// </summary> 
        /// <exception cref="System.ArgumentNullException"><paramref name="provider"> is null.</exception>
        /// <param name="provider">The type of the attribute to get.</param>
        /// <returns>The fetched attributes, empty if none are found..</returns>
        public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider provider) where T : Attribute {
            return Argument.NotNull(provider).GetCustomAttributes(true).OfType<T>();
        }

    }

}