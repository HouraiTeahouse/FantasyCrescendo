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

using System;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.Events {
    /// <summary> Abstract class for creating objects in response to an event. </summary>
    /// <typeparam name="T"> the object type to spawn </typeparam>
    /// <typeparam name="TEvent"> the event type to respond to </typeparam>
    public abstract class AbstractFactoryEventHandler<T, TEvent> :
        EventHandlerBehaviour<TEvent> where T : Object {
        public event Action<T, TEvent> OnCreate;

        /// <summary> Events callback. Called whenever said event is published by the mediator. </summary>
        /// <param name="eventArgs"> the event arguments </param>
        protected sealed override void OnEvent(TEvent eventArgs) {
            if (!ShouldCreate(eventArgs))
                return;
            T instance = Create(eventArgs);
            OnCreate.SafeInvoke(instance, eventArgs);
        }

        /// <summary> Gets whether an instance should be spawned in response to an event or not. </summary>
        /// <param name="eventArgs"> the event arguments </param>
        /// <returns> whether a new instance should be spawned. </returns>
        protected virtual bool ShouldCreate(TEvent eventArgs) {
            return eventArgs != null;
        }

        /// <summary> Creates a new instance of type <typeparamref name="T" />
        /// </summary>
        /// <param name="eventArgs"> the event arguments </param>
        /// <returns> a new instance of <typeparamref name="T" /> </returns>
        protected abstract T Create(TEvent eventArgs);
    }
}