using System;
using System.Collections;
using System.Collections.Generic;

namespace HouraiTeahouse {

    /// <summary> Map between the values of a Enum and the values of another type. Essentially works as a dictionary
    /// prepopulated with all the values of a enum that cannot have keys added or removed. </summary>
    /// <typeparam name="TEnum"> the type of the enum to use </typeparam>
    /// <typeparam name="TValue"> the type to map the enum to. </typeparam>
    public class EnumMap<TEnum, TValue> : IEnumerable<TValue>
        where TEnum : struct, IComparable, IFormattable, IConvertible {

        // The backing dictionary
        readonly Dictionary<TEnum, TValue> _map;

        /// <summary> Initializes a new EnumMap instance. Populates it with keys of all the values of the specified enumereation. </summary>
        /// <exception cref="ArgumentException"> <typeparamref name="TEnum" /> is not a Enum type. </exception>
        public EnumMap() {
            Type enumType = typeof(TEnum);
            if (!enumType.IsEnum)
                throw new ArgumentException("Cannot create an EnumMap from a non-enum type!");
            _map = new Dictionary<TEnum, TValue>();
            foreach (TEnum enumVal in Enum.GetValues(enumType))
                _map[enumVal] = default(TValue);
        }

        /// <summary> Indexer for accessing mapped values given a valid value of the source enum. </summary>
        /// <param name="enumVal"> value of the source enum </param>
        /// <returns> the mapped value </returns>
        public TValue this[TEnum enumVal] {
            get { return _map[enumVal]; }
            set { _map[enumVal] = value; }
        }

        /// <summary> Gets the number of elements stored in this enum map. Is always equal to the number of possible values the
        /// enum can be. </summary>
        public int Count {
            get { return _map.Count; }
        }

        #region IEnumerable Implementation

        public IEnumerator<TValue> GetEnumerator() {
            foreach (KeyValuePair<TEnum, TValue> kvp in _map)
                yield return kvp.Value;
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        #endregion
    }

}