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

namespace HouraiTeahouse {

    public delegate float Modifier<in T>(T source, float damage);

    /// <summary> A ordered list of modifiers. </summary>
    public class ModifierList : PriorityList<Func<float, float>> {
        /// <summary> Modifies a base value based on the provided modifiers. </summary>
        /// <param name="baseValue"> the base value before modification1 </param>
        /// <returns> the final modified value </returns>
        public float Modifiy(float baseValue) {
            if (Count <= 0)
                return baseValue;
            float value = baseValue;
            foreach (Func<float, float> mod in this)
                value = mod(value);
            return value;
        }
    }

    /// <summary> An ordered list of modifiers. </summary>
    /// <typeparam name="T"> the modifier parameter type </typeparam>
    public class ModifierList<T> : PriorityList<Modifier<T>> {
        /// <summary> Modifies a base value based on the provided modifiers </summary>
        /// <param name="source"> the modifier argument </param>
        /// <param name="baseValue"> the value before modifiication </param>
        /// <returns> the modified value </returns>
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
