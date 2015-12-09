using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Hourai {

    public class EnumMap<TEnum, TValue> : IEnumerable<TValue>
        where TEnum : struct, IComparable, IFormattable, IConvertible {

        private Dictionary<TEnum, TValue> map;

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

        public int Count {
            get { return map.Count; }
        }

        public IEnumerator<TValue> GetEnumerator() {
            foreach (KeyValuePair<TEnum, TValue> kvp in map)
                yield return kvp.Value;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }


}
