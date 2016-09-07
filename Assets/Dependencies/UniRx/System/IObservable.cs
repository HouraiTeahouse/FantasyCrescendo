using System;

#if !NETFX_CORE

namespace UniRx {

    public interface IObservable<T> {

        IDisposable Subscribe(IObserver<T> observer);

    }

}

#endif

namespace UniRx {

    public interface IGroupedObservable<TKey, TElement> : IObservable<TElement> {

        TKey Key { get; }

    }

}