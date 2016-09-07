#if !NETFX_CORE

using System;

namespace UniRx {

    public interface IObserver<T> {

        void OnCompleted();
        void OnError(Exception error);
        void OnNext(T value);

    }

}

#endif