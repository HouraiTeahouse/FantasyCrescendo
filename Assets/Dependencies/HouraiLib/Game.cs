using System;
using UnityEngine;

namespace Hourai {
    
    public abstract class Game<T> : MonoBehaviour where T : Game<T> {

        private static T _instance;

        public static T Instance {
            get {
                if (_instance == null)
                    _instance = FindObjectOfType<T>();
                return _instance;
            }
        }

        [SerializeField, Range(0f, 5f)]
        private float _timeScale = 1f;

        private static bool _paused;

        public static bool Paused {
            get { return _paused; }
            set {
                bool oldVal = _paused;
                _paused = value;
                Time.timeScale = _paused ? 0f : Instance._timeScale;
                if (oldVal != value && OnPause != null)
                    OnPause();
            }
        }

        public static float TimeScale {
            get { return Instance ? Instance._timeScale : Time.timeScale; }
            set {
                float old = TimeScale;
                if (Instance)
                    Instance._timeScale = value;
                else
                    Time.timeScale = value;
                if (old != TimeScale && OnTimeScaleChange != null)
                    OnTimeScaleChange();
            }
        }

        public static event Action OnPause;
        public static event Action OnTimeScaleChange;

        protected virtual void Awake() {
            _instance = this as T;
            Time.timeScale = _timeScale;
        }

        protected virtual void Update() {
            Time.timeScale = Paused ? 0f : TimeScale;
        }

    }

}
