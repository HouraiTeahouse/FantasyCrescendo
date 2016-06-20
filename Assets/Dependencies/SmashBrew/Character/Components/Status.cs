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
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    public abstract class Status : AbstractCharacterComponent {
        float _duration = Mathf.Infinity;

        public float EllapsedTime { get; private set; }

        public float Duration {
            get { return _duration; }
            set {
                _duration = value;
                enabled = EllapsedTime < Duration;
            }
        }

        public static T Apply<T>(GameObject target, float duration = -1f)
            where T : Status {
            if (!target)
                throw new ArgumentNullException("target");
            var instance = target.GetOrAddComponent<T>();
            instance.StartStatus(duration);
            return instance;
        }

        public static T Apply<T>(Component target, float duration = -1f)
            where T : Status {
            if (!target)
                throw new ArgumentNullException("target");
            return Apply<T>(target.gameObject, duration);
        }

        protected virtual void Start() { enabled = false; }

        protected virtual float GetDeltaTime() { return DeltaTime; }

        void Update() {
            float dt = GetDeltaTime();
            EllapsedTime += dt;
            OnStatusUpdate(dt);
            enabled = EllapsedTime < Duration;
        }

        void OnEnable() {
            EllapsedTime = 0f;
            OnStatusStart();
        }

        void OnDisable() { OnStatusEnd(); }

        public void StartStatus(float duration = -1) {
            if (duration > 0)
                Duration = duration;
            enabled = true;
        }

        protected virtual void OnStatusStart() { }

        protected virtual void OnStatusUpdate(float dt) { }

        protected virtual void OnStatusEnd() { }
    }
}