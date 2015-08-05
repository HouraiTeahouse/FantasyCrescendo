using System;

namespace Crescendo.API {

    public class GlobalCallbacks : Singleton<GlobalCallbacks> {

        public static event Action OnUpdate;
        public static event Action OnLateUpdate;
        public static event Action OnFixedUpdate;
        public static event Action<int> OnLoad;
        public static event Action OnApplicationFocused;
        public static event Action OnApplicationUnfocused;
        public static event Action OnApplicationExit;

        private void Update() {
            OnUpdate.SafeInvoke();
        }

        private void LateUpdate() {
            OnLateUpdate.SafeInvoke();
        }

        private void FixedUpdate() {
            OnFixedUpdate.SafeInvoke();
        }

        private void OnApplicationFocus(bool focus) {
            if (focus)
                OnApplicationFocused.SafeInvoke();
            else
                OnApplicationUnfocused.SafeInvoke();
        }

        private void OnApplicationQuit() {
            OnApplicationExit.SafeInvoke();
        }

        private void OnLevelWasLoaded(int level) {
            OnLoad.SafeInvoke(level);
        }

    }

}