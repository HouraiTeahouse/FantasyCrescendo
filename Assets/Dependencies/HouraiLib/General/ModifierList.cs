using System;

namespace HouraiTeahouse {

    public delegate TBase Modifier<in TSource, TBase>(TSource source, TBase damage);

    /// <summary> An ordered list of modifiers. </summary>
    /// <typeparam name="TSource"> the modifier parameter type </typeparam>
    /// <typeparam name="TBase"> the type that is being modified </typeparam>
    public class ModifierList<TSource, TBase> : PriorityList<Modifier<TSource, TBase>> {

        /// <summary> Modifies a base value based on the provided modifiers </summary>
        /// <param name="source"> the modifier argument </param>
        /// <param name="baseValue"> the value before modifiication </param>
        /// <returns> the modified value </returns>
        public TBase Modifiy(TSource source, TBase baseValue) {
            if (Count <= 0)
                return baseValue;
            TBase value = baseValue;
            foreach (Modifier<TSource, TBase> mod in this)
                value = mod(source, value);
            return value;
        }

    }

}