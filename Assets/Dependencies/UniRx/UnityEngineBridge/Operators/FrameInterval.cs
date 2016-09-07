using System;
using UnityEngine;

namespace UniRx.Operators {

    internal class FrameIntervalObservable<T> : OperatorObservableBase<FrameInterval<T>> {

        class FrameInterval : OperatorObserverBase<T, FrameInterval<T>> {

            int lastFrame;

            public FrameInterval(IObserver<FrameInterval<T>> observer, IDisposable cancel) : base(observer, cancel) {
                lastFrame = Time.frameCount;
            }

            public override void OnNext(T value) {
                int now = Time.frameCount;
                int span = now - lastFrame;
                lastFrame = now;

                observer.OnNext(new FrameInterval<T>(value, span));
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

        readonly IObservable<T> source;

        public FrameIntervalObservable(IObservable<T> source) : base(source.IsRequiredSubscribeOnCurrentThread()) {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<FrameInterval<T>> observer, IDisposable cancel) {
            return source.Subscribe(new FrameInterval(observer, cancel));
        }

    }

}