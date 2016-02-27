using System;

namespace HouraiTeahouse {
    
    public delegate float Modifier<T>(T source, float damage);

    /// <summary>
    /// A ordered list of modifiers.
    /// </summary>
    public class ModifierList : PriorityList<Func<float, float>> {

        /// <summary>
        /// Modifies a base value based on the provided modifiers.
        /// </summary>
        /// <param name="baseValue">the base value before modification1</param>
        /// <returns>the final modified value</returns>
        public float Modifiy(float baseValue) {
            if (Count <= 0)
                return baseValue;
            float value = baseValue;
            foreach (Func<float, float> mod in this)
                value = mod(value);
            return value;
        }

    }

    /// <summary>
    /// An ordered list of modifiers.
    /// </summary>
    /// <typeparam name="T">the modifier parameter type</typeparam>
    public class ModifierList<T> : PriorityList<Modifier<T>> {
     
        /// <summary>
        /// Modifies a base value based on the provided modifiers
        /// </summary>  
        /// <param name="source">the modifier argument</param>
        /// <param name="baseValue">the value before modifiication</param>
        /// <returns>the modified value</returns>
        public float Modifiy(T source, float baseValue) {
            if (Count <= 0)
                return baseValue;
            float value = baseValue;
            foreach (Modifier<T> mod in this)
                value = mod(source, value);
            return value;
        }

    }

}
