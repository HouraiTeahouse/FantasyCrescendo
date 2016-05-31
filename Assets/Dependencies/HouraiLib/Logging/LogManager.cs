using UnityEngine;

namespace HouraiTeahouse {

    public sealed class LogManager : Singleton<LogManager> {

        [SerializeField]
        private LogSettings _settings;

        protected override void Awake() {
            base.Awake();
            Log.Settings = _settings;
        }
    }
}
