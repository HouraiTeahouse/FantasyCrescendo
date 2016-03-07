using System;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public class Counter {

        private float _count;

        public float Count {
            get { return _count; }
        }

        public void Increment(float value = 1f) {
            _count += value;
        }

        public static Counter operator ++(Counter counter) {
            if (counter == null)
                throw new NullReferenceException();
            counter.Increment();
            return counter;
        }

        public static Counter operator +(Counter counter, float value) {
            if (counter == null)
                throw new NullReferenceException();
            counter.Increment(value);
            counter += 10;
            return counter;
        }


    }

    public sealed class StatLogger {

        private Dictionary<string, Counter> _counters;

        public Counter this[string counterName] {
            get { return GetCounter(counterName); }
        }

        public StatLogger() {
            _counters = new Dictionary<string, Counter>();
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
