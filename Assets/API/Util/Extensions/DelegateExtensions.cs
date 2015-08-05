using System;
using System.Linq;

namespace Crescendo.API {

    /// <summary>
    /// A set of utility extension functions to make it easier to use various delegate functions.
    /// SafeInvoke is a short hand way to invoke delegates that also includes a null check.
    /// </summary>
    public static class DelegateExtensions {

        /// <summary>
        /// Checks whether a Action delegate contains a specific handler.
        /// Note this function uses Linq, and has a expected worst case runtime of O(n) with respect
        /// to the size of the invocation list of the supplied delegate.
        /// </summary>
        /// <param name="del"></param>
        /// <param name="handler"></param>
        /// <returns>whether or not the delegate contains the specifed handler</returns>
        public static bool HasHandler(this Action del, Action handler) {
            return del.GetInvocationList().Contains(handler);
        }

        /// <summary>
        /// Invokes the delegate if it's not null
        /// </summary>
        public static void SafeInvoke(this Action del) {
            if (del != null)
                del();
        }

        /// <summary>
        /// Invokes the delegate if it's not null with the specified argument
        /// </summary>
        public static void SafeInvoke<T>(this Action<T> del, T value) {
            if (del != null)
                del(value);
        }

        /// <summary>
        /// Invokes the delegate if it's not null with the specified arguments
        /// </summary>
        public static void SafeInvoke<T, T1>(this Action<T, T1> del, T value1, T1 value2) {
            if (del != null)
                del(value1, value2);
        }

        /// <summary>
        /// Invokes the delegate if it's not null with the specified arguments
        /// </summary>
        public static void SafeInvoke<T, T1, T2>(this Action<T, T1, T2> del, T value1, T1 value2, T2 value3) {
            if (del != null)
                del(value1, value2, value3);
        }

        /// <summary>
        /// Invokes the delegate if it's not null with the specified arguments
        /// </summary>
        public static void SafeInvoke<T, T1, T2, T3>(this Action<T, T1, T2, T3> del,
                                                     T value1,
                                                     T1 value2,
                                                     T2 value3,
                                                     T3 value4) {
            if (del != null)
                del(value1, value2, value3, value4);
        }

        /// <summary>
        /// Invokes the delegate if it's not null
        /// </summary>
        public static TReturn SafeInvoke<TReturn>(this Func<TReturn> del) {
            return del == null ? del() : default(TReturn);
        }

        /// <summary>
        /// Invokes the delegate if it's not null
        /// </summary>
        public static TReturn SafeInvoke<TReturn, T0>(this Func<T0, TReturn> del, T0 arg0) {
            return del == null ? del(arg0) : default(TReturn);
        }

        /// <summary>
        /// Invokes the delegate if it's not null
        /// </summary>
        public static TReturn SafeInvoke<TReturn, T0, T1>(this Func<T0, T1, TReturn> del, T0 arg0, T1 arg1) {
            return del == null ? del(arg0, arg1) : default(TReturn);
        }

        /// <summary>
        /// Invokes the delegate if it's not null
        /// </summary>
        public static TReturn SafeInvoke<TReturn, T0, T1, T2>(this Func<T0, T1, T2, TReturn> del,
                                                              T0 arg0,
                                                              T1 arg1,
                                                              T2 arg2) {
            return del == null ? del(arg0, arg1, arg2) : default(TReturn);
        }

        /// <summary>
        /// Invokes the delegate if it's not null
        /// </summary>
        public static TReturn SafeInvoke<TReturn, T0, T1, T2, T3>(this Func<T0, T1, T2, T3, TReturn> del,
                                                                  T0 arg0,
                                                                  T1 arg1,
                                                                  T2 arg2,
                                                                  T3 arg3) {
            return del == null ? del(arg0, arg1, arg2, arg3) : default(TReturn);
        }

        /// <summary>
        /// Returns true if this delegate doesn't have any handlers
        /// </summary>
        public static bool IsEmpty(this Delegate del) {
            if (del == null)
                return true;
            var list = del.GetInvocationList();
            return list.Length == 1 && list[0].Target == null;
        }

    }

}