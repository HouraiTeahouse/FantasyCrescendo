using System;
using System.Collections;
using System.Collections.Generic;

namespace UniRx {

    public static class AotSafeExtensions {

        public static IEnumerable<T> AsSafeEnumerable<T>(this IEnumerable<T> source) {
            IEnumerator e = ((IEnumerable) source).GetEnumerator();
            using (e as IDisposable) {
                while (e.MoveNext()) {
                    yield return (T) e.Current;
                }
            }
        }

    }

}