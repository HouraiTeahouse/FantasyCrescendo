using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HouraiTeahouse.SmashBrew.States {

    public class StateController<T, TContext> where T : State<TContext> {

        public T DefaultState { get; private set; }
        public T CurrentState { get; private set; }
        readonly HashSet<T> _states;
        public ReadOnlyCollection<T> States { get; private set; }

        public event Action<T, T> OnStateChange;

        internal StateController(StateControllerBuilder<T, TContext> builder) {
            DefaultState = builder.DefaultState;
            CurrentState = DefaultState;
            _states = builder._states;
            States = new ReadOnlyCollection<T>(builder.States.ToArray());
        }

        public void SetState(T state) {
            Argument.NotNull(state);
            if (!_states.Contains(state))
                throw new ArgumentException("Cannot set a state to a state not within the controller.");
            var oldState = CurrentState;
            CurrentState = state; 
            if (oldState != CurrentState)
                OnStateChange.SafeInvoke(oldState, CurrentState);
        }

        public T UpdateState(TContext context) {
            var nextState = CurrentState.EvaluateTransitions(context) as T;
            if (nextState != null)
                SetState(nextState);
            return CurrentState;
        }

        public void ResetState() {
            SetState(DefaultState);
        }

    }

}

