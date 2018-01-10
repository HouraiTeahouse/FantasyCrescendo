using System;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo {

    public static class StateExtensions {

        public static State<T> AddTransition<T>(this State<T> state, Func<State<T>> transition) {
            Argument.NotNull(state);
            Argument.NotNull(transition);
            return state.AddTransition(ctx => transition());
        }

        public static T AddTransition<T, TContext>(this T src, State<TContext> dst, Func<TContext, bool> predicate)
                                                   where T : State<TContext> {
            Argument.NotNull(src);
            Argument.NotNull(dst);
            Argument.NotNull(predicate);
            src.AddTransition(context => (predicate(context)) ? dst : null);
            return src;
        }

        public static IEnumerable<T> AddTransitions<T, TContext>(this IEnumerable<T> states, 
                                                                Func<TContext, State<TContext>> transition)
                                                                where T : State<TContext> {
            foreach (T state in Argument.NotNull(states).Distinct()) {
                if (state != null)
                    state.AddTransition(transition);
            }
            return states;
        }

        public static IEnumerable<T> AddTransitions<T, TContext>(this IEnumerable<T> srcs,
                                                                State<TContext> dst,
                                                                Func<TContext, bool> predicate)
                                                                where T : State<TContext> {
            return srcs.AddTransitions<T, TContext>(context => (predicate(context)) ? dst :  null);
        }

    }
}

