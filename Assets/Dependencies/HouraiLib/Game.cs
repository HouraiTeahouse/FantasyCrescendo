using System;
using System.Collections;
using System.Linq;
using UnityConstants;
using UnityEngine;

namespace Hourai {
    
    public abstract class Game : Singleton<Game> {

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
                if (old != TimeScale)
                    OnTimeScaleChange();
            }
        }

        // All the things that require an actual instance
        #region Global Callbacks
        
        public static event Action OnGameStart;
        public static event Action OnPause;
        public static event Action OnTimeScaleChange;

        public static event Action OnUpdate;
        public static event Action OnLateUpdate;
        public static event Action OnFixedUpdate;
        public static event Action<int> OnLoad;
        public static event Action OnApplicationFocused;
        public static event Action OnApplicationUnfocused;
        public static event Action OnApplicationExit;

        private void Awake() {
            Time.timeScale = _timeScale;
            base.Awake();
        }

        private void Start() {
            if(OnGameStart != null)
                OnGameStart();
        }

        private void Update() {
            if(OnUpdate != null)
                OnUpdate();
            Time.timeScale = Paused ? 0f : TimeScale;
        }

        private void LateUpdate() {
            if(OnLateUpdate != null)
                OnLateUpdate();
        }

        private void FixedUpdate() {
            if(OnFixedUpdate != null)
                OnFixedUpdate();
        }

        private void OnApplicationFocus(bool focus) {
            if (focus)
                if(OnApplicationFocused != null)
                    OnApplicationFocused();
            else
                if(OnApplicationUnfocused != null)
                    OnApplicationUnfocused();
        }

        private void OnApplicationQuit() {
            if(OnApplicationExit != null)
                OnApplicationExit();
        }

        private void OnLevelWasLoaded(int level) {
            if(OnLoad != null)
                OnLoad(level);
        }

        #endregion
        
        #region Global Coroutines

        public static Coroutine StaticCoroutine(IEnumerator coroutine) {
            if(Instance == null)
                throw new InvalidOperationException("Cannot start a static coroutine without a Game instance.");
            if(coroutine == null)
                throw new ArgumentNullException("coroutine");
            return Instance.StartCoroutine(coroutine);
        }

        public static Coroutine StaticCoroutine(IEnumerable coroutine) {
            if(Instance == null)
                throw new InvalidOperationException("Cannot start a static coroutine without a Game instance.");
            if (coroutine == null)
                throw new ArgumentNullException("coroutine");
            return StaticCoroutine(coroutine.GetEnumerator());
        }

        #endregion

        public static bool IsPlayer(Component obj) {
            return obj.CompareTag(Tags.Player);
        }

        public static GameObject FindPlayer() {
            return GameObject.FindGameObjectWithTag(Tags.Player);
        }

        public static GameObject[] FindPlayers() {
            return GameObject.FindGameObjectsWithTag(Tags.Player);
        }

        public static GameObject FindGUI() {
            return GameObject.FindGameObjectWithTag(Tags.GUI);
        }
    }

}
