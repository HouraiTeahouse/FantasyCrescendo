using System;

namespace Genso.API {

    public class GlobalCallbacks : Singleton<GlobalCallbacks> {

        public static event Action OnUpdate;
        public static event Action OnLateUpdate;
        public static event Action OnFixedUpdate;
        public static event Action<int> OnLoad;
        public static event Action OnApplicationFocused;
        public static event Action OnApplicationUnfocused;
        public static event Action OnApplicationExit;

        void Update() {
            OnUpdate.SafeInvoke();
        }

        void LateUpdate() {
            OnLateUpdate.SafeInvoke();
        }

        void FixedUpdate() {
            OnFixedUpdate.SafeInvoke();
        }

        void OnApplicationFocus(bool focus) {
            if(focus)
                OnApplicationFocused.SafeInvoke();
            else 
                OnApplicationUnfocused.SafeInvoke();
        }

        void OnApplicationQuit() {
            OnApplicationExit.SafeInvoke();
        }

        void OnLevelWasLoaded(int level) {
            OnLoad.SafeInvoke(level);
        }

    }


}