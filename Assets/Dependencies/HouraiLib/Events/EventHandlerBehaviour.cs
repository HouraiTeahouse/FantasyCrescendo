// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace HouraiTeahouse.Events {
    /// <summary> An abstract class for MonoBehaviours that handle events published by Mediators. </summary>
    /// <typeparam name="T"> the event type to subscribe to </typeparam>
    public abstract class EventHandlerBehaviour<T> : HouraiBehaviour {
        Mediator _eventManager;

        /// <summary> Gets or sets the event manager the event handler is subscribed to. </summary>
        public Mediator EventManager {
            get { return _eventManager; }
            set {
                if (_eventManager != null)
                    _eventManager.Unsubscribe<T>(OnEvent);
                _eventManager = value;
                if (_eventManager != null)
                    _eventManager.Subscribe<T>(OnEvent);
            }
        }

        /// <summary> Unity callback. Called on object instantiation. </summary>
        protected override void Awake() {
            base.Awake();
            _eventManager = GlobalMediator.Instance;
            _eventManager.Subscribe<T>(OnEvent);
        }

        /// <summary> Unity callback. Called on object destruction. </summary>
        /// >
        protected virtual void OnDestroy() {
            if (_eventManager != null)
                _eventManager.Unsubscribe<T>(OnEvent);
        }

        /// <summary> Events callback. </summary>
        /// <param name="EventArgs"> event arguments </param>
        protected abstract void OnEvent(T EventArgs);
    }
}