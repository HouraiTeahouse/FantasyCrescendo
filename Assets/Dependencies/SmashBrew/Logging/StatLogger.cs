using System.Collections.Generic;

namespace HouraiTeahouse.SmashBrew {
    public class Counter {
        private float _count;

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
        private readonly Dictionary<string, Counter> _counters;

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
