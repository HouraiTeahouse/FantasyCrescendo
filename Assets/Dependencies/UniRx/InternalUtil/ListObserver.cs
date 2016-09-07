using System;

namespace UniRx.InternalUtil {

    public class ListObserver<T> : IObserver<T> {

        private readonly ImmutableList<IObserver<T>> _observers;

        public ListObserver(ImmutableList<IObserver<T>> observers) { _observers = observers; }

        public void OnCompleted() {
            IObserver<T>[] targetObservers = _observers.Data;
            for (var i = 0; i < targetObservers.Length; i++) {
                targetObservers[i].OnCompleted();
            }
        }

        public void OnError(Exception error) {
            IObserver<T>[] targetObservers = _observers.Data;
            for (var i = 0; i < targetObservers.Length; i++) {
                targetObservers[i].OnError(error);
            }
        }

        public void OnNext(T value) {
            IObserver<T>[] targetObservers = _observers.Data;
            for (var i = 0; i < targetObservers.Length; i++) {
                targetObservers[i].OnNext(value);
            }
        }

        internal IObserver<T> Add(IObserver<T> observer) { return new ListObserver<T>(_observers.Add(observer)); }

        internal IObserver<T> Remove(IObserver<T> observer) {
            int i = Array.IndexOf(_observers.Data, observer);
            if (i < 0)
                return this;

            if (_observers.Data.Length == 2) {
                return _observers.Data[1 - i];
            }
            else {
                return new ListObserver<T>(_observers.Remove(observer));
            }
        }

    }

    public class EmptyObserver<T> : IObserver<T> {

        public static readonly EmptyObserver<T> Instance = new EmptyObserver<T>();

        EmptyObserver() { }

        public void OnCompleted() { }

        public void OnError(Exception error) { }

        public void OnNext(T value) { }

    }

    public class ThrowObserver<T> : IObserver<T> {

        public static readonly ThrowObserver<T> Instance = new ThrowObserver<T>();

        ThrowObserver() { }

        public void OnCompleted() { }

        public void OnError(Exception error) { throw error; }

        public void OnNext(T value) { }

    }

    public class DisposedObserver<T> : IObserver<T> {

        public static readonly DisposedObserver<T> Instance = new DisposedObserver<T>();

        DisposedObserver() { }

        public void OnCompleted() { throw new ObjectDisposedException(""); }

        public void OnError(Exception error) { throw new ObjectDisposedException(""); }

        public void OnNext(T value) { throw new ObjectDisposedException(""); }

    }

}