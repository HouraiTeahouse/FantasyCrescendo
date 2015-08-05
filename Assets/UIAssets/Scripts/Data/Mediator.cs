using System.Collections.Generic;

public class Command {

}

public delegate void MediatorCallback<T>(T c) where T : Command;

public class Mediator {

    //make sure you're using the System.Collections.Generic namespace
    private Dictionary<System.Type, System.Delegate> _subscribers = new Dictionary<System.Type, System.Delegate>();

    public void Subscribe<T>(MediatorCallback<T> callback) where T : Command {
        if (callback == null)
            throw new System.ArgumentNullException("callback");
        var tp = typeof (T);
        if (_subscribers.ContainsKey(tp))
            _subscribers[tp] = System.Delegate.Combine(_subscribers[tp], callback);
        else
            _subscribers.Add(tp, callback);
    }

    public void DeleteSubscriber<T>(MediatorCallback<T> callback) where T : Command {
        if (callback == null)
            throw new System.ArgumentNullException("callback");
        var tp = typeof (T);
        if (_subscribers.ContainsKey(tp)) {
            var d = _subscribers[tp];
            d = System.Delegate.Remove(d, callback);
            if (d == null)
                _subscribers.Remove(tp);
            else
                _subscribers[tp] = d;
        }
    }

    public void Publish<T>(T c) where T : Command {
        var tp = typeof (T);
        if (_subscribers.ContainsKey(tp))
            _subscribers[tp].DynamicInvoke(c);
    }

}