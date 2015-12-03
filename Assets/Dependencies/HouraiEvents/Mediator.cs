using System;
using System.Collections.Generic;

namespace Hourai.Events {

    public interface IEvent {
    }

    public class Mediator {

        //make sure you're using the System.Collections.Generic namespace
        private Dictionary<Type, Delegate> _subscribers = new Dictionary<Type, Delegate>();

        public void Subscribe<T>(Action<T> callback) where T : IEvent {
            if (callback == null)
                throw new ArgumentNullException("callback");
            Type tp = typeof (T);
            if (_subscribers.ContainsKey(tp))
                _subscribers[tp] = Delegate.Combine(_subscribers[tp], callback);
            else
                _subscribers.Add(tp, callback);
        }

        public void Unsubscribe<T>(Action<T> callback) where T : IEvent {
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

        public void Publish(IEvent e) { 
            Type tp = e.GetType();
            if (_subscribers.ContainsKey(tp))
                _subscribers[tp].DynamicInvoke(e);
        }

    }

}