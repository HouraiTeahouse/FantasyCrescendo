using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Hourai {

    public class EnumMap<TEnum, TValue> : IEnumerable<TValue>
        where TEnum : struct, IComparable, IFormattable, IConvertible {

        private readonly Dictionary<TEnum, TValue> map;

        public EnumMap() {
            Type enumType = typeof(TEnum);
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("Cannot create an EnumMap from a non-enum type!");
            map = new Dictionary<TEnum, TValue>();
            foreach (TEnum enumVal in Enum.GetValues(enumType)) {
                map[enumVal] = default(TValue);
            }
        }

        public TValue this[TEnum enumVal] {
            get { return map[enumVal]; }
            set { map[enumVal] = value; }
        }

        public int Count => map.Count;

        public IEnumerator<TValue> GetEnumerator() {
            return map.Select(kvp => kvp.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }


}
