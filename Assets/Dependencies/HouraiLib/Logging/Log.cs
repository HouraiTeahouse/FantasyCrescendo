using System;
using UnityEngine;

namespace HouraiTeahouse {

    [Serializable]
    public class LogTypeSettings {

        public bool Enabled;
        public StackTraceLogType StackTrace;

    }

    [Serializable]
    public class LogSettings {

        [SerializeField]
        string _timeFormat = "MM-dd-yy HH:mm:ss";

        [SerializeField]
        LogTypeSettings _info = new LogTypeSettings {Enabled = true, StackTrace = StackTraceLogType.None};

        [SerializeField]
        LogTypeSettings _debug = new LogTypeSettings {Enabled = true, StackTrace = StackTraceLogType.None};

        [SerializeField]
        LogTypeSettings _warning = new LogTypeSettings {Enabled = true, StackTrace = StackTraceLogType.None};

        [SerializeField]
        LogTypeSettings _error = new LogTypeSettings {Enabled = true, StackTrace = StackTraceLogType.None};

        public LogTypeSettings Info {
            get { return _info; }
        }

        public LogTypeSettings Debug {
            get { return _debug; }
        }

        public LogTypeSettings Error {
            get { return _error; }
        }

        public LogTypeSettings Warning {
            get { return _warning; }
        }

        public string TimeFormat {
            get { return _timeFormat; }
            set { _timeFormat = value; }
        }

    }

    public static class Log {

        static LogSettings _settings = new LogSettings();

        public static LogSettings Settings {
            get { return _settings; }
            set { _settings = Check.NotNull(value); }
        }

        public static void Info(object source, params object[] objs) {
#if UNITY_EDITOR
            const string format = "<color=green>[Info]</color> ({1}) - {0}";
#else
            const string format = "[Info] ({1}) - {0}";
#endif
            WriteLog(format, _settings.Info, LogType.Log, source, objs);
        }

        public static void Debug(object source, params object[] objs) {
#if UNITY_EDITOR
            const string format = "<color=blue>[Debug]</color> ({1}) - {0}";
#else
            const string format = "[Debug] ({1}) - {0}";
#endif
            WriteLog(format, _settings.Debug, LogType.Log, source, objs);
        }

        public static void Warning(object source, params object[] objs) {
#if UNITY_EDITOR
            const string format = "<color=yellow>[Warning]</color> ({1}) -  {0}";
#else
            const string format = "[Warning] ({1}) - {0}";
#endif
            WriteLog(format, _settings.Warning, LogType.Warning, source, objs);
        }

        public static void Error(object source, params object[] objs) {
#if UNITY_EDITOR
            const string format = "<color=red>[Error]</color> ({1}) - {0}";
#else
            const string format = "[Error] {0}";
#endif
            WriteLog(format, _settings.Error, LogType.Error, source, objs);
        }

        static void WriteLog(string format, LogTypeSettings settings, LogType log, object source, params object[] objs) {
            if (!settings.Enabled)
                return;
            StackTraceLogType logType = Application.GetStackTraceLogType(log);
            Application.SetStackTraceLogType(log, settings.StackTrace);
            var output = source as string;
            if (output != null)
                output = output.With(objs);
            else
                output = source == null ? "Null" : source.ToString();
#if UNITY_EDITOR
            UnityEngine.Debug.logger.LogFormat(log, format, output, DateTime.Now.ToString(_settings.TimeFormat));
#else
            System.Console.WriteLine(format, output, DateTime.Now.ToString(_settings.TimeFormat));
#endif
            Application.SetStackTraceLogType(log, logType);
        }

    }

}