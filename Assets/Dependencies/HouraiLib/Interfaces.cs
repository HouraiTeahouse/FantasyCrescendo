using System;

namespace HouraiTeahouse {

    //TODO: Document
    public interface IRegistrar<in T> {

        void Register(T obj);
        bool Unregister(T obj);

    }

    //TODO: Document
    public interface IResettable {

        void OnReset();

    }

    public interface IUniqueEntity<out T> {

        T ID { get; }

    }

    public interface INameable {

        string Name { get; }

    }

    public interface IResolvable {

        void Resolve();

    }

    public interface IResolvable<in T> : IResolvable {

        void Resolve(T value);

    }

    public interface IRejectable {

        void Reject(Exception error);

    }

}
