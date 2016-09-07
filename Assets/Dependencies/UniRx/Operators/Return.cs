using System;

namespace UniRx.Operators {

    internal class ReturnObservable<T> : OperatorObservableBase<T> {

        class Return : OperatorObserverBase<T, T> {

            public Return(IObserver<T> observer, IDisposable cancel) : base(observer, cancel) { }

            public override void OnNext(T value) {
                try {
                    observer.OnNext(value);
                } catch {
                    Dispose();
                    throw;
                }
            }

            public override void OnError(Exception error) {
                try {
                    observer.OnError(error);
                } finally {
                    Dispose();
                }
            }

            public override void OnCompleted() {
                try {
                    observer.OnCompleted();
                } finally {
                    Dispose();
                }
            }

        }

        readonly T value;
        readonly IScheduler scheduler;

        public ReturnObservable(T value, IScheduler scheduler) : base(scheduler == Scheduler.CurrentThread) {
            this.value = value;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel) {
            observer = new Return(observer, cancel);

            if (scheduler == Scheduler.Immediate) {
                observer.OnNext(value);
                observer.OnCompleted();
                return Disposable.Empty;
            }
            else {
                return scheduler.Schedule(() => {
                    observer.OnNext(value);
                    observer.OnCompleted();
                });
            }
        }

    }

    internal class ImmediateReturnObservable<T> : IObservable<T>, IOptimizedObservable<T> {

        readonly T value;

        public ImmediateReturnObservable(T value) { this.value = value; }

        public IDisposable Subscribe(IObserver<T> observer) {
            observer.OnNext(value);
            observer.OnCompleted();
            return Disposable.Empty;
        }

        public bool IsRequiredSubscribeOnCurrentThread() { return false; }

    }

    internal class ImmutableReturnUnitObservable : IObservable<Unit>, IOptimizedObservable<Unit> {

        internal static ImmutableReturnUnitObservable Instance = new ImmutableReturnUnitObservable();

        ImmutableReturnUnitObservable() { }

        public IDisposable Subscribe(IObserver<Unit> observer) {
            observer.OnNext(Unit.Default);
            observer.OnCompleted();
            return Disposable.Empty;
        }

        public bool IsRequiredSubscribeOnCurrentThread() { return false; }

    }

    internal class ImmutableReturnTrueObservable : IObservable<bool>, IOptimizedObservable<bool> {

        internal static ImmutableReturnTrueObservable Instance = new ImmutableReturnTrueObservable();

        ImmutableReturnTrueObservable() { }

        public IDisposable Subscribe(IObserver<bool> observer) {
            observer.OnNext(true);
            observer.OnCompleted();
            return Disposable.Empty;
        }

        public bool IsRequiredSubscribeOnCurrentThread() { return false; }

    }

    internal class ImmutableReturnFalseObservable : IObservable<bool>, IOptimizedObservable<bool> {

        internal static ImmutableReturnFalseObservable Instance = new ImmutableReturnFalseObservable();

        ImmutableReturnFalseObservable() { }

        public IDisposable Subscribe(IObserver<bool> observer) {
            observer.OnNext(false);
            observer.OnCompleted();
            return Disposable.Empty;
        }

        public bool IsRequiredSubscribeOnCurrentThread() { return false; }

    }

}