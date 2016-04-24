using UnityEngine;

namespace HouraiTeahouse {

    public sealed class LogManager : Singleton<LogManager> {

        [SerializeField]
        private bool _showInfoLogs = true;

        [SerializeField]
        private bool _showDebugLogs = true;

        [SerializeField]
        private bool _showWarningLogs = true;

        [SerializeField]
        private bool _showErrorLogs = true;

        [SerializeField]
        private StackTraceLogType _infoStackTrace = StackTraceLogType.ScriptOnly;

        [SerializeField]
        private StackTraceLogType _debugStackTrace = StackTraceLogType.ScriptOnly;

        [SerializeField]
        private StackTraceLogType _warningStackTrace = StackTraceLogType.ScriptOnly;

        [SerializeField]
        private StackTraceLogType _errorStackTrace = StackTraceLogType.ScriptOnly;

        protected override void Awake() {
            base.Awake();
            Log.ShowInfo = _showInfoLogs;
            Log.ShowDebug = _showDebugLogs;
            Log.ShowWarning = _showWarningLogs;
            Log.ShowError = _showErrorLogs;

            Log.StackTraceInfo = _infoStackTrace;
            Log.StackTraceDebug = _debugStackTrace;
            Log.StackTraceWarning = _warningStackTrace;
            Log.StackTraceError = _errorStackTrace;

        }
    }
}
