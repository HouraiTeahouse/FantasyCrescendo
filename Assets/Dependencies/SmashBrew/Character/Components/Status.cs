using UnityEngine;
using System.Collections;
using System.Timers;

namespace Hourai.SmashBrew {
    
    public abstract class Status : CharacterComponent {
        
        private float _duration = Mathf.Infinity;

        public bool Started { get; private set; }

        public float EllapsedTime { get; private set; }

        public float Duration {
            get { return _duration; }
            set {
                Duration = value;
                if (EllapsedTime > Duration)
                    EndStatus();
            }
        }

        protected sealed override void Start() {
            base.Start();
            Started = false;
            EllapsedTime = 0f;
            OnStatusStart();
        }

        void Update() {
            if (Started)
                EllapsedTime += Util.dt;
            OnStatusUpdate();
            if (EllapsedTime > Duration)
                EndStatus();
        }

        public void StartStatus(float duration = -1) {
            if(duration > 0)
                Duration = duration;
            Started = true;
        }

        public void EndStatus() {
            OnStatusEnd();
            Destroy(this);
        }

        protected abstract void OnStatusStart();
        protected abstract void OnStatusUpdate();
        protected abstract void OnStatusEnd();

    }

}
