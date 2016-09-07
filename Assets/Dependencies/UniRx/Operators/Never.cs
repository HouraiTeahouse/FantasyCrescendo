using System;

namespace UniRx.Operators {

    internal class NeverObservable<T> : OperatorObservableBase<T> {

        public NeverObservable() : base(false) { }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel) {
            return Disposable.Empty;
        }

    }

    internal class ImmutableNeverObservable<T> : IObservable<T>, IOptimizedObservable<T> {

        internal static ImmutableNeverObservable<T> Instance = new ImmutableNeverObservable<T>();

        public IDisposable Subscribe(IObserver<T> observer) { return Disposable.Empty; }

        public bool IsRequiredSubscribeOnCurrentThread() { return false; }

    }

}