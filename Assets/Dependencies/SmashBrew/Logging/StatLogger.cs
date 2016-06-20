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

using System.Collections.Generic;

namespace HouraiTeahouse.SmashBrew {
    public class Counter {
        float _count;

        public float Count {
            get { return _count; }
        }

        public Counter Increment(float value = 1f) {
            _count += value;
            return this;
        }

        public static Counter operator ++(Counter counter) {
            return Check.NotNull("counter", counter).Increment();
        }

        public static Counter operator +(Counter counter, float value) {
            return Check.NotNull("counter", counter).Increment(value);
        }
    }

    public sealed class StatLogger {
        readonly Dictionary<string, Counter> _counters;

        public StatLogger() { _counters = new Dictionary<string, Counter>(); }

        public Counter this[string counterName] {
            get { return GetCounter(counterName); }
        }

        public Counter GetCounter(string counterName) {
            Counter counter;
            if (_counters.ContainsKey(counterName))
                counter = _counters[counterName];
            else {
                counter = new Counter();
                _counters[counterName] = counter;
            }
            return counter;
        }
    }
}