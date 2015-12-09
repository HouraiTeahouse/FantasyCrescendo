using System;
using System.Collections.Generic;

namespace Hourai.Events {

    /// <summary>
    /// A generalized object that encapsulates the interactions between multiple objects.
    /// Meant to be used as either local or global event managers.
    /// </summary>
    public class Mediator {

        // Maps types of events to a set of handlers
        private readonly Dictionary<Type, Delegate> _subscribers = new Dictionary<Type, Delegate>();

        /// <summary>
        /// Adds a listener/handler for a specific event type.
        /// </summary>
        /// <remarks>
        /// The event dispatch system does <b>not</b> work with polymorphism and inhertance. If A is a subclass of B, it cannot recieve
        /// events of type B, only events of type A.
        /// </remarks>
        /// <typeparam name="T">the type of event to listen for</typeparam>
        /// <param name="callback">the handler to call when an event of type <typeparamref name="T"/> is published.</param>
        public void Subscribe<T>(Action<T> callback) {
            if (callback == null)
                throw new ArgumentNullException("callback");
            Type tp = typeof (T);
            if (_subscribers.ContainsKey(tp))
                _subscribers[tp] = Delegate.Combine(_subscribers[tp], callback);
            else
                _subscribers.Add(tp, callback);
        }

        /// <summary>
        /// Removes a listener from a specfic event type.
        /// </summary>
        /// <typeparam name="T">the type of event to remove the handler from</typeparam>
        /// <param name="callback">the handler to remove</param>
        public void Unsubscribe<T>(Action<T> callback) {
            if (callback == null)
                throw new ArgumentNullException("callback");
            Type eventType = typeof (T);
            if (!_subscribers.ContainsKey(eventType))
                return;
            Delegate d = _subscribers[eventType];
            d = Delegate.Remove(d, callback);
            if (d == null)
                _subscribers.Remove(eventType);
            else
                _subscribers[eventType] = d;
        }

        /// <summary>
        /// Publishes a new event.
        /// </summary>
        /// <remarks>
        /// Execution is immediate. All handler code will be executed before returning from this method.
        /// There is no specification saying that the event object may not be mutated. The object may be altered after execution.
        /// </remarks>
        /// <param name="e">the event object</param>
        public void Publish(object e) { 
            Type tp = e.GetType();
            if (_subscribers.ContainsKey(tp))
                _subscribers[tp].DynamicInvoke(e);
        }

    }

}