using System;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse.SmashBrew.States {

    public static class StateExtensions {

        public static State<T> AddTransition<T>(this State<T> state, Func<State<T>> transition) {
            Argument.NotNull(state);
            Argument.NotNull(transition);
            return state.AddTransition(ctx => transition());
        }

        public static State<T> AddTransition<T>(this State<T> src, State<T> dst, Func<T, bool> predicate) {
            Argument.NotNull(src);
            Argument.NotNull(dst);
            Argument.NotNull(predicate);
            return src.AddTransition(context => (predicate(context)) ? dst : null);
        }

        public static IEnumerable<State<T>> AddTransition<T>(this IEnumerable<State<T>> states, 
                                                             Func<T, State<T>> transition) {
            foreach (State<T> state in Argument.NotNull(states).Distinct()) {
                if (state != null)
                    state.AddTransition(transition);
            }
            return states;
        }

        public static IEnumerable<State<T>> AddTransition<T>(this IEnumerable<State<T>> srcs,
                                                    State<T> dst,
                                                    Func<T, bool> predicate) {
            return srcs.AddTransition(context => (predicate(context)) ? dst :  null);
        }

    }
}

