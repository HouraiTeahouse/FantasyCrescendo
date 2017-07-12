using System;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse.SmashBrew.States {

    public class StateControllerBuilder<T, TContext> where T : State<TContext> {

        internal readonly HashSet<T> _states;
        public T DefaultState { get; set; }
        public IEnumerable<T> States {
            get { return _states.Select(x => x); }
        }

        public StateControllerBuilder() {
            _states = new HashSet<T>();
        }

        public StateController<T, TContext> Build() {
            if (DefaultState == null)
                throw new InvalidOperationException();
            if (!_states.Contains(DefaultState))
                throw new InvalidOperationException();
            return new StateController<T, TContext>(this);
        }

        public StateControllerBuilder<T, TContext> WithDefaultState(T state) {
            if (state == null)
                throw new ArgumentNullException("state");
            if (!_states.Contains(state))
                AddState(state);
            DefaultState = state;
            return this;
        }

        public StateControllerBuilder<T, TContext> AddState(T state) {
            if (state == null)
                throw new ArgumentNullException("state");
            if (_states.Contains(state))
                throw new ArgumentException("States cannot be added multiple times: {0}".With(state));
            _states.Add(state);
            return this;
        }

    }

}

