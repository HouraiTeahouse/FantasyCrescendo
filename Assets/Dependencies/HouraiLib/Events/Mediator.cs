using System;
using System.Collections.Generic;

namespace HouraiTeahouse {

    /// <summary> A generalized object that encapsulates the interactions between multiple objects. Meant to be used as either
    /// local or global event managers. </summary>
    public class Mediator {

        public delegate void Event<in T>(T arg);

        public static readonly Mediator Global = new Mediator();

        // Maps types of events to a set of handlers
        readonly Dictionary<Type, Delegate> _subscribers;
        readonly Dictionary<Type, Type[]> _typeCache;

        /// <summary> Initializes an instance of Mediator </summary>
        public Mediator() {
            _subscribers = new Dictionary<Type, Delegate>();
            _typeCache = new Dictionary<Type, Type[]>();
        }

        // Internal only, for testing
        internal Mediator(Dictionary<Type, Delegate> subscribers) {
            Argument.NotNull(subscribers);
            _subscribers = new Dictionary<Type, Delegate>(subscribers);
            _typeCache = new Dictionary<Type, Type[]>();
        }

        /// <summary> Adds a listener/handler for a specific event type. </summary>
        /// <remarks> The event dispatch system does <b> not </b> work with polymorphism and inhertance. If A is a subclass of B,
        /// it cannot recieve events of type B, only events of type A. </remarks>
        /// <typeparam name="T"> the type of event to listen for </typeparam>
        /// <param name="callback"> the handler to call when an event of type <typeparamref name="T" /> is published. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="callback" /> is null </exception>
        public void Subscribe<T>(Event<T> callback) {
            Argument.NotNull(callback);
            Type tp = typeof(T);
            if (_subscribers.ContainsKey(tp))
                _subscribers[tp] = Delegate.Combine(_subscribers[tp], callback);
            else
                _subscribers.Add(tp, callback);
        }

        /// <summary> Removes a listener from a specfic event type. </summary>
        /// <typeparam name="T"> the type of event to remove the handler from </typeparam>
        /// <param name="callback"> the handler to remove </param>
        /// <exception cref="ArgumentNullException"> <paramref name="callback" /> is null </exception>
        public void Unsubscribe<T>(Event<T> callback) {
            Argument.NotNull(callback);
            Type eventType = typeof(T);
            if (!_subscribers.ContainsKey(eventType))
                return;
            Delegate d = _subscribers[eventType];
            d = Delegate.Remove(d, callback);
            if (d == null)
                _subscribers.Remove(eventType);
            else
                _subscribers[eventType] = d;
        }

        /// <summary> Publishes a new event. </summary>
        /// <remarks> Execution is immediate. All handler code will be executed before returning from this method. There is no
        /// specification saying that the event object may not be mutated. The object may be altered after execution. </remarks>
        /// <param name="evnt"> the event object </param>
        /// <exception cref="ArgumentNullException"> <paramref name="evnt" /> is null </exception>
        public void Publish(object evnt) {
            Argument.NotNull(evnt);
            Type[] typeSet = GetEventTypes(evnt.GetType());
            for (var i = 0; i < typeSet.Length; i++)
                if (_subscribers.ContainsKey(typeSet[i]))
                    _subscribers[typeSet[i]].DynamicInvoke(evnt);
        }

        /// <summary> Gets the count of subscribers to certain type of event. </summary>
        /// <typeparam name="T"> the type of event to check for. </typeparam>
        /// <returns> how many subscribers said event has. </returns>
        public int GetCount<T>() { return GetCount(typeof(T)); }

        /// <summary> Gets the count of subscribers to certain type of event. </summary>
        /// <param name="type"> the type of event to check for. </param>
        /// <returns> how many subscribers said event has </returns>
        public int GetCount(Type type) {
            Argument.NotNull(type);
            return _subscribers.ContainsKey(type) ? _subscribers[type].GetInvocationList().Length : 0;
        }

        /// <summary> Removes all subscribers from a single event. </summary>
        /// <typeparam name="T"> the type of event to remove </typeparam>
        /// <returns> whether it was removed or not </returns>
        public bool Reset<T>() { return Reset(typeof(T)); }

        /// <summary> Removes all subscribers from a single event. </summary>
        /// <param name="type"> the type of event to remove </param>
        /// <returns> whether it was removed or not </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="type" /> is null </exception>
        public bool Reset(Type type) { return _subscribers.Remove(Argument.NotNull(type)); }

        /// <summary> Removes all subscribers from all events. </summary>
        /// <returns> whether any events were removed </returns>
        public bool ResetAll() {
            bool success = _subscribers.Count > 0;
            _subscribers.Clear();
            return success;
        }

        Type[] GetEventTypes(Type type) {
            Argument.NotNull(type);
            if (!_typeCache.ContainsKey(type)) {
                Type currentType = type;
                var ancestors = new List<Type>(type.GetInterfaces());
                while (currentType != null) {
                    ancestors.Add(currentType);
                    currentType = currentType.BaseType;
                }
                _typeCache.Add(type, ancestors.ToArray());
            }
            return _typeCache[type];
        }

    }

}
