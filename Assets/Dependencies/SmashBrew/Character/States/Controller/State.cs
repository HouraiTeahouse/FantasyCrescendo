using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HouraiTeahouse.SmashBrew.States {

    public abstract class State<T> {

        readonly List<Func<T, State<T>>> _transitions;
        readonly ReadOnlyCollection<Func<T, State<T>>> _transitionCollection;
        public ReadOnlyCollection<Func<T, State<T>>> Transitions {
            get { return _transitionCollection; }
        }

        protected State() {
            _transitions = new List<Func<T, State<T>>>();
            _transitionCollection = new ReadOnlyCollection<Func<T, State<T>>>(_transitions);
        }

        public State<T> AddTransition(Func<T, State<T>> transition) {
            Argument.NotNull(transition);
            _transitions.Add(transition);
            return this;
        }

        public State<T> EvaluateTransitions(T context) {
            return _transitions.Select(func => func(context))
                .FirstOrDefault(state => state != null && state.IsActive(context));
        }

        public virtual bool IsActive(T context) { return true; }

    }

}
