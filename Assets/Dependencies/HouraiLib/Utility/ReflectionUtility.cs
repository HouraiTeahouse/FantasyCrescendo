using System;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse {

    public static class ReflectionUtilty {

        public static IEnumerable<Type> AllTypes {
            get {
                return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                       from type in assembly.GetTypes()
                       select type;
            }
        }

        public static IEnumerable<Type> IsAssignableFrom(this IEnumerable<Type> types, Type baseType) {
            return types.Where(baseType.IsAssignableFrom);
        }

        public static IEnumerable<Type> ConcreteClasses(this IEnumerable<Type> types) {
            return types.Where(t => !t.IsAbstract && t.IsClass);
        }

        public static IEnumerable<KeyValuePair<Type, T>> WithAttribute<T>(this IEnumerable<Type> types,
                                                                          bool inherit = true) {
            Type attributeType = typeof(T);
            foreach (Type type in types) {
                IEnumerable<T> attributes = type.GetCustomAttributes(attributeType, inherit).OfType<T>();
                foreach (T attribute in attributes) {
                    yield return new KeyValuePair<Type, T>(type, attribute);
                }
            }
        }

    }

}