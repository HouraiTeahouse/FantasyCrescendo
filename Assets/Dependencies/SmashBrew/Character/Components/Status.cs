using UnityEngine;
using System.Collections;
using System.Timers;

namespace Hourai.SmashBrew {
    
    public abstract class Status : CharacterComponent {
        
        private float _duration = Mathf.Infinity;

        public float EllapsedTime { get; private set; }

        public float Duration {
            get { return _duration; }
            set {
                _duration = value;
                enabled = EllapsedTime < Duration;
            }
        }

        void Awake() {
            enabled = false;
        }

        void FixedUpdate() {
            EllapsedTime += FixedDeltaTime;
            OnStatusUpdate(FixedDeltaTime);
            enabled = EllapsedTime < Duration;
        }

        void OnEnable() {
            EllapsedTime = 0f;
            OnStatusStart();
        }

        void OnDisable() {
            OnStatusEnd();
        }

        public void StartStatus(float duration = -1) {
            if(duration > 0)
                Duration = duration;
            enabled = true;
        }

        protected virtual void OnStatusStart() {}
        protected virtual void OnStatusUpdate(float dt) {}
        protected virtual void OnStatusEnd() {}

    }

}
