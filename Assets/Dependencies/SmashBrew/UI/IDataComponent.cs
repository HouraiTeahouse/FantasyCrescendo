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
using System.Collections.Generic;

namespace HouraiTeahouse.SmashBrew {
    /// <summary> An interface for defining Player driven components. </summary>
    public interface IDataComponent<in T> {
        /// <summary> Sets the data for the IDataComponent </summary>
        /// <param name="data"> the new data to set </param>
        void SetData(T data);
    }

    /// <summary> Extension methods for IDataComponent and collections of IDataComponents </summary>
    public static class IDataComponentExtensions {
        /// <summary> Sets the data on all elements in a collection of IDataComponent </summary>
        /// <typeparam name="T"> the type of data </typeparam>
        /// <param name="enumeration"> the colleciton of IDataComponents </param>
        /// <exception cref="ArgumentNullException"> <paramref name="enumeration" /> is null </exception>
        /// <param name="data"> the data to be set </param>
        public static void SetData<T>(
            this IEnumerable<IDataComponent<T>> enumeration,
            T data) {
            foreach (IDataComponent<T> dataComponent in
                Check.NotNull(enumeration).IgnoreNulls())
                dataComponent.SetData(data);
        }
    }
}
