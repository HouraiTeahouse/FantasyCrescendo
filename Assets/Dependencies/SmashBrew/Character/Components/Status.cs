using System;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public abstract class Status : CharacterComponent {

        float _duration = Mathf.Infinity;

        public float EllapsedTime { get; private set; }

        public float Duration {
            get { return _duration; }
            set {
                _duration = Math.Abs(value);
                enabled = EllapsedTime < Duration;
            }
        }

        public static T Apply<T>(GameObject target, float duration = -1f) where T : Status {
            var instance = Check.NotNull(target).GetOrAddComponent<T>();
            instance.StartStatus(duration);
            return instance;
        }

        public static T Apply<T>(Component target, float duration = -1f) where T : Status {
            return Apply<T>(Check.NotNull(target).gameObject, duration);
        }

        protected override void Start() {
            base.Start();
            enabled = false;
        }

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