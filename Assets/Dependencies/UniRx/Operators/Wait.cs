using System;
using System.Threading;

namespace UniRx.Operators {

    internal class Wait<T> : IObserver<T> {

        static readonly TimeSpan InfiniteTimeSpan = new TimeSpan(0, 0, 0, 0, -1); // from .NET 4.5

        readonly IObservable<T> source;
        readonly TimeSpan timeout;

        ManualResetEvent semaphore;

        bool seenValue = false;
        T value = default(T);
        Exception ex = default(Exception);

        public Wait(IObservable<T> source, TimeSpan timeout) {
            this.source = source;
            this.timeout = timeout;
        }

        public void OnNext(T value) {
            seenValue = true;
            this.value = value;
        }

        public void OnError(Exception error) {
            ex = error;
            semaphore.Set();
        }

        public void OnCompleted() { semaphore.Set(); }

        public T Run() {
            semaphore = new ManualResetEvent(false);
            using (source.Subscribe(this)) {
                bool waitComplete = timeout == InfiniteTimeSpan ? semaphore.WaitOne() : semaphore.WaitOne(timeout);

                if (!waitComplete) {
                    throw new TimeoutException("OnCompleted not fired.");
                }
            }

            if (ex != null)
                throw ex;
            if (!seenValue)
                throw new InvalidOperationException("No Elements.");

            return value;
        }

    }

}