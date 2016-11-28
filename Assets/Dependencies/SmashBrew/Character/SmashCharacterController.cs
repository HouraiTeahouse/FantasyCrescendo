using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace HouraiTeahouse.SmashBrew {

    internal class ActionSet {
        public readonly Func<StateSet, bool> StateCheck;
        public readonly ICharacterAction Action;
        public ActionSet(Func<StateSet, bool> stateCheck, ICharacterAction action) {
            StateCheck = Argument.NotNull(stateCheck);
            Action = Argument.NotNull(action);
        }
    }

    public class SmashCharacterControllerBuildder {

        readonly Dictionary<Type, ICharacterState> _states;
        readonly Dictionary<Type, ActionSet> _actions;

        public SmashCharacterControllerBuildder() {
            _states = new Dictionary<Type, ICharacterState>();
        }

        public SmashCharacterControllerBuildder WithState(ICharacterState state) {
            _states.Add(state.GetType(), state);
            return this;
        }

        public SmashCharacterControllerBuildder WithStates(IEnumerable<ICharacterState> states) {
            foreach (ICharacterState state in states)
                WithState(state);
            return this;
        }

        public SmashCharacterControllerBuildder AddAction<T>(Func<StateSet, bool> stateCheck)
            where T : ICharacterAction {
            Argument.NotNull(stateCheck);
            var type = typeof(T);
            if (_actions.ContainsKey(type))
                throw new InvalidOperationException();
            _actions[type] = new ActionSet(stateCheck, Activator.CreateInstance<T>());
            return this;
        }

        public SmashCharacterController Build() {
            return new SmashCharacterController(_states, _actions.Values);
        }

    }

    public class StateSet : IEnumerable<ICharacterState> {

        readonly Dictionary<Type, ICharacterState> _states;

        public StateSet(IDictionary<Type, ICharacterState> states) {
            _states = new Dictionary<Type, ICharacterState>(states);
        }

        public T Get<T>() where T : ICharacterState {
            var type = typeof(T);
            if (_states.ContainsKey(type))
                return (T)_states[type];
            return default(T);
        }

        public Func<T> CachedGet<T>() where T : ICharacterState { return new Func<T>(Get<T>).Memoize(); }

        public IEnumerator<ICharacterState> GetEnumerator() { return _states.Values.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

    }

    public class SmashCharacterController {

        readonly StateSet _states;
        readonly IEnumerable<ActionSet> _actions;

        public StateSet States {
            get { return _states; }
        }
        public IEnumerable<ICharacterAction> Actions {
            get { return _actions.Select(a => a.Action); }
        }
        public IEnumerable<ICharacterAction> ValidActions {
            get { return _actions.Where(a => a.StateCheck(States)).Select(a => a.Action); }
        }

        internal SmashCharacterController(Dictionary<Type, ICharacterState> states, IEnumerable<ActionSet> actions) {
            _states = new StateSet(Argument.NotNull(states));
            _actions = actions.ToArray();
        }

        public void Execute() {
            //TODO(james7132): Evaluate peformance of this
            foreach (var action in ValidActions) {
                //TODO(james7132): Add input evaluation here
                action.Execute(States);
            }
        }

    }

}
