// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException(
                    "Cannot create an EnumMap from a non-enum type!");
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
