using System;
using System.Threading;
using UniRx.InternalUtil;

namespace UniRx.Operators {

    public abstract class OperatorObserverBase<TSource, TResult> : IDisposable, IObserver<TSource> {

        protected internal volatile IObserver<TResult> observer;
        IDisposable cancel;

        public OperatorObserverBase(IObserver<TResult> observer, IDisposable cancel) {
            this.observer = observer;
            this.cancel = cancel;
        }

        public void Dispose() {
            observer = EmptyObserver<TResult>.Instance;
            IDisposable target = Interlocked.Exchange(ref cancel, null);
            if (target != null) {
                target.Dispose();
            }
        }

        public abstract void OnNext(TSource value);

        public abstract void OnError(Exception error);

        public abstract void OnCompleted();

    }

}