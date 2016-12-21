using System;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters.Statuses {

    public abstract class Status : NetworkBehaviour, ICharacterState {

        //TODO(james7132): Synchronize over the network

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
            var instance = Argument.NotNull(target).GetOrAddComponent<T>();
            instance.StartStatus(duration);
            return instance;
        }

        public static T Apply<T>(Component target, float duration = -1f) where T : Status {
            return Apply<T>(Argument.NotNull(target).gameObject, duration);
        }

        void Start() {
            enabled = false;
        }


        void Update() {
            float dt = Time.deltaTime;
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

        public void ResetState() {
            //TODO(james7132): Implement
        }

    }

}
