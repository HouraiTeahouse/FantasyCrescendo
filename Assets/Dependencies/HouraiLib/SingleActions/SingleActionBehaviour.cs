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

using UnityEngine;
using UnityEngine.EventSystems;

namespace HouraiTeahouse {
    /// <summary> A MonoBehaviour that does only one action, which can be set to do so on a variety of triggers. </summary>
    public abstract class SingleActionBehaviour : MonoBehaviour, ISubmitHandler {
        enum Type {
            // Do the action on instantiation 
            Instantiation,
            // Do the action in response to being enabled
            OnEnable,
            // Do the action in response to being disabled 
            OnDisable,
            // Do the action on the first frame
            Start,
            // Do the action once per frame
            Update,
            // Do the action once per fixed timestep
            FixedUpdate,
            // Do the action on physical collision with an object
            Collision,
            // Do the action when the object is destroyed
            Destruction,
            // Do the action in response to a a Submit action
            UIEvent,
        }

        [SerializeField]
        Type _trigger;

        /// <summary>
        ///     <see cref="ISubmitHandler.OnSubmit" />
        /// </summary>
        public void OnSubmit(BaseEventData eventData) {
            if (_trigger == Type.UIEvent)
                Action();
        }

        /// <summary> Unity callback. Called on object instantiation. </summary>
        protected virtual void Awake() {
            if (_trigger == Type.Instantiation)
                Action();
        }

        /// <summary> Unity callback. Called once before the object's first frame. </summary>
        protected virtual void Start() {
            if (_trigger == Type.Start)
                Action();
        }

        /// <summary> Unity callback. Called on contact with another object with a trigger collider. </summary>
        protected virtual void OnTriggerEnter(Collider other) {
            if (_trigger == Type.Collision)
                Action();
        }

        /// <summary> Unity callback. Called once per frame. </summary>
        protected virtual void Update() {
            if (_trigger == Type.Update)
                Action();
        }

        /// <summary> Unity callback. Called when the object is enabled. </summary>
        protected virtual void OnEnable() {
            if (_trigger == Type.OnEnable)
                Action();
        }

        /// <summary> Unity callback. Called when the object is no longer enabled. </summary>
        protected virtual void OnDisable() {
            if (_trigger == Type.OnDisable)
                Action();
        }

        /// <summary> Unity callback. Called repeatedly on a fixed timestep. </summary>
        protected virtual void FixedUpdate() {
            if (_trigger == Type.FixedUpdate)
                Action();
        }

        /// <summary> Unity callback. Called on object destruction. </summary>
        protected virtual void OnDestroy() {
            if (_trigger == Type.Destruction)
                Action();
        }

        /// <summary> The action to execute. </summary>
        protected abstract void Action();
    }
}