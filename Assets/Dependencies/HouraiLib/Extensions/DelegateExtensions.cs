using System;
using System.Collections.Generic;

namespace HouraiTeahouse {

    public static class DelegateExtensions {

        /// <summary> Safely invokes a delegate. If it is null, it will not be invoked. </summary>
        /// <param name="action"> the delegate to invoke </param>
        public static void SafeInvoke(this Action action) {
            if (action != null)
                action();
        }

        /// <summary> Safely invokes a delegate. If it is null, it will not be invoked. </summary>
        /// <typeparam name="T"> the argument parameter </typeparam>
        /// <param name="action"> the delegate to invoke </param>
        /// <param name="arg"> the argument </param>
        public static void SafeInvoke<T>(this Action<T> action, T arg) {
            if (action != null)
                action(arg);
        }

        /// <summary> Safely invokes a delegate. If it is null, it will not be invoked. </summary>
        /// <typeparam name="T1"> the type of the first parameter </typeparam>
        /// <typeparam name="T2"> the type of the second parameter </typeparam>
        /// <param name="action"> the delegate to invoke </param>
        /// <param name="arg1"> the first paramter </param>
        /// <param name="arg2"> the second parameter </param>
        public static void SafeInvoke<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2) {
            if (action != null)
                action(arg1, arg2);
        }

        /// <summary> Safely invokes a delegate. If it is null, it will not be invoked. </summary>
        /// <typeparam name="T1"> the type of the first parameter </typeparam>
        /// <typeparam name="T2"> the type of the second parameter </typeparam>
        /// <typeparam name="T3"> the type of the third paramter </typeparam>
        /// <param name="action"> the delegate to invoke </param>
        /// <param name="arg1"> the first paramter </param>
        /// <param name="arg2"> the second parameter </param>
        /// <param name="arg3"> the third parameter </param>
        public static void SafeInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3) {
            if (action != null)
                action(arg1, arg2, arg3);
        }

        /// <summary> Safely invokes a delegate. If it is null, it will not be invoked. </summary>
        /// <typeparam name="T1"> the type of the first parameter </typeparam>
        /// <typeparam name="T2"> the type of the second parameter </typeparam>
        /// <typeparam name="T3"> the type of the third paramter </typeparam>
        /// <typeparam name="T4"> the type of the fourth paramter </typeparam>
        /// <param name="action"> the delegate to invoke </param>
        /// <param name="arg1"> the first paramter </param>
        /// <param name="arg2"> the second parameter </param>
        /// <param name="arg3"> the third parameter </param>
        /// <param name="arg4"> the fourth parameter </param>
        public static void SafeInvoke<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action,
                                                      T1 arg1,
                                                      T2 arg2,
                                                      T3 arg3,
                                                      T4 arg4) {
            if (action != null)
                action(arg1, arg2, arg3, arg4);
        }

        /// <summary> Memoizes a function. Makes it easy to call an expensive funciton multiple times. This function assumes the
        /// result is immutable and does not change over time. Note that the results passed back by the function will not be
        /// garbage collected until the retunred memoized function falls out of scope. </summary>
        /// <typeparam name="T"> the return type of the function </typeparam>
        /// <param name="func"> the function to memoize </param>
        /// <exception cref="ArgumentNullException"> <paramref name="func" /> is null </exception>
        /// <returns> the memoized function </returns>
        public static Func<T> Memoize<T>(this Func<T> func) {
            Check.NotNull(func);
            object cache = null;
            return delegate {
                if (cache == null)
                    cache = func();
                return (T) cache;
            };
        }

        /// <summary> Memoizes a function. Makes it easy to call an expensive funciton multiple times. This function assumes the
        /// result is immutable and does not change over time. Note that the results passed back by the function will not be
        /// garbage collected until the retunred memoized function falls out of scope. </summary>
        /// <typeparam name="T"> the function's argument type </typeparam>
        /// <typeparam name="TResult"> the return type of the function </typeparam>
        /// <param name="func"> the function to memoize </param>
        /// <exception cref="ArgumentNullException"> <paramref name="func" /> is null </exception>
        /// <returns> the memoized function </returns>
        public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> func) {
            Check.NotNull(func);
            var cache = new Dictionary<T, TResult>();
            return delegate(T val) {
                if (!cache.ContainsKey(val))
                    cache[val] = func(val);
                return cache[val];
            };
        }

        /// <summary> Memoizes a function. Makes it easy to call an expensive funciton multiple times. This function assumes the
        /// result is immutable and does not change over time. Note that the results passed back by the function will not be
        /// garbage collected until the retunred memoized function falls out of scope. </summary>
        /// <typeparam name="T1"> the function's first argument type </typeparam>
        /// <typeparam name="T2"> the function's second argument type </typeparam>
        /// <typeparam name="TResult"> the return type of the function </typeparam>
        /// <param name="func"> the function to memoize </param>
        /// <exception cref="ArgumentNullException"> <paramref name="func" /> is null </exception>
        /// <returns> the memoized function </returns>
        public static Func<T1, T2, TResult> Memoize<T1, T2, TResult>(this Func<T1, T2, TResult> func) {
            Check.NotNull(func);
            var cache = new Table2D<T1, T2, TResult>();
            return delegate(T1 arg1, T2 arg2) {
                if (!cache.ContainsKey(arg1, arg2))
                    cache[arg1, arg2] = func(arg1, arg2);
                return cache[arg1, arg2];
            };
        }

    }

}