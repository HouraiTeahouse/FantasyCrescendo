using System;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.Events {
    /// <summary>
    /// Abstract class for creating objects in response to an event.
    /// </summary>
    /// <typeparam name="T">the object type to spawn</typeparam>
    /// <typeparam name="TEvent">the event type to respond to</typeparam>
    public abstract class AbstractFactoryEventHandler<T, TEvent> : EventHandlerBehaviour<TEvent>
        where T : Object {
        public event Action<T, TEvent> OnCreate;

        /// <summary>
        /// Event callback. Called whenever said event is published by the mediator.
        /// </summary>
        /// <param name="eventArgs">the event arguments</param>
        protected sealed override void OnEvent(TEvent eventArgs) {
            if (!ShouldCreate(eventArgs))
                return;
            T instance = Create(eventArgs);
            if (OnCreate != null)
                OnCreate(instance, eventArgs);
        }

        /// <summary>
        /// Gets whether an instance should be spawned in response to an event or not.
        /// </summary>
        /// <param name="eventArgs">the event arguments</param>
        /// <returns>whether a new instance should be spawned.</returns>
        protected virtual bool ShouldCreate(TEvent eventArgs) {
            return eventArgs != null;
        }

        /// <summary>
        /// Creates a new instance of type <typeparamref name="T"/>
        /// </summary>
        /// <param name="eventArgs">the event arguments</param>
        /// <returns>a new instance of <typeparamref name="T"/></returns>
        protected abstract T Create(TEvent eventArgs);
    }
}