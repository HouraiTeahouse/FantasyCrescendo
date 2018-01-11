using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo {

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

        /// <summary>
        /// Sets the current state of the StateController to the provided state.
        /// This will not invoke the <see cref="OnStateChange"/> event.
        /// For a version that doesn't invoke the event, use <see cref="ChangeState"/>
        /// </summary>
        /// <param name="state">the state to change to</param>
        /// <exception cref="System.ArgumentException"><paramref cref="state"> is not a state of the StateController</exception>
        /// <exception cref="System.ArgumentNullException"><paramref cref="state"> is null</exception>
        public void SetState(T state) {
            Argument.NotNull(state);
            if (!_states.Contains(state))
                throw new ArgumentException("Cannot set a state to a state not within the controller.");
            CurrentState = state; 
        }

        /// <summary>
        /// Changes the current state of the state controller to the provided state.
        /// This is suggested to be used for what is normally seen as a state transition.
        /// This will invoke the <see cref="OnStateChange"/> event.
        /// For a version that doesn't invoke the event, use <see cref="SetState"/>
        /// </summary>
        /// <param name="state">the state to change to</param>
        /// <exception cref="System.ArgumentException"><paramref cref="state">/ is not a state of the StateController</exception>
        /// <exception cref="System.ArgumentNullException"><paramref cref="state"/> is null</exception>
        public void ChangeState(T state) {
            var oldState = CurrentState;
            SetState(state);
            if (oldState != CurrentState && OnStateChange != null)
                OnStateChange(oldState, CurrentState);
        }

        /// <summary>
        /// Evaluates possible state transitions, and updates <see cref="CurrentState"/> approriately.
        /// </summary>
        /// <param name="context">the context object for evaluating the chnage against</param>
        /// <returns>the new current state</returns>
        public T UpdateState(TContext context) {
            return UpdateState(context, false);
        }

        T UpdateState(TContext context, bool passthrough) {
            T nextState;
            if (passthrough)
                nextState = CurrentState.Passthrough(context) as T;
            else
                nextState = CurrentState.EvaluateTransitions(context) as T;
            if (nextState != null) {
                switch (nextState.GetEntryPolicy(context)) {
                    case StateEntryPolicy.Normal:
                        ChangeState(nextState);
                        break;
                    case StateEntryPolicy.Passthrough:
                        ChangeState(nextState);
                        UpdateState(context, true);
                        break;
                    case StateEntryPolicy.Blocked:
                        break;
                }
            }
            return CurrentState;
        }

        /// <summary>
        /// Resets <see cref="CurrentState"/> to 
        /// </summary>
        public void ResetState() {
            ChangeState(DefaultState);
        }

    }

}

