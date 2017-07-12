using System;

namespace HouraiTeahouse {

    public class Cacheable<T> {

        readonly Func<T> _generator;
        T value;
        bool _computed;

        public Cacheable(Func<T> generator) {
            _computed = false;
            _generator = Argument.NotNull(generator);
        }

        public T Get() {
            if (_computed)
                return value;
            value = _generator();
            _computed = true;
            return value;
        }

    }

}

