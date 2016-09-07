using System;

namespace UniRx {

    public sealed class SerialDisposable : IDisposable, ICancelable {

        readonly object gate = new object();
        IDisposable current;
        bool disposed;

        public IDisposable Disposable {
            get { return current; }
            set {
                var shouldDispose = false;
                IDisposable old = default(IDisposable);
                lock (gate) {
                    shouldDispose = disposed;
                    if (!shouldDispose) {
                        old = current;
                        current = value;
                    }
                }
                if (old != null) {
                    old.Dispose();
                }
                if (shouldDispose && value != null) {
                    value.Dispose();
                }
            }
        }

        public bool IsDisposed {
            get {
                lock (gate) {
                    return disposed;
                }
            }
        }

        public void Dispose() {
            IDisposable old = default(IDisposable);

            lock (gate) {
                if (!disposed) {
                    disposed = true;
                    old = current;
                    current = null;
                }
            }

            if (old != null) {
                old.Dispose();
            }
        }

    }

}