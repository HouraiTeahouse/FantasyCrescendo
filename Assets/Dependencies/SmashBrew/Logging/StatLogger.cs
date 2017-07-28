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

        public static Counter operator ++(Counter counter) { return Argument.NotNull(counter).Increment(); }

        public static Counter operator +(Counter counter, float value) {
            return Argument.NotNull(counter).Increment(value);
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