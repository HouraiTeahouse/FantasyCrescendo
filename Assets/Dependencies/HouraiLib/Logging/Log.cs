using System;
using System.Collections.Generic;
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
        string _timeFormat = "HH:mm:ss";

        [SerializeField]
        LogTypeSettings _info = new LogTypeSettings {Enabled = true, StackTrace = StackTraceLogType.None};

        [SerializeField]
        LogTypeSettings _debug = new LogTypeSettings {Enabled = true, StackTrace = StackTraceLogType.ScriptOnly};

        [SerializeField]
        LogTypeSettings _warning = new LogTypeSettings {Enabled = true, StackTrace = StackTraceLogType.ScriptOnly};

        [SerializeField]
        LogTypeSettings _error = new LogTypeSettings {Enabled = true, StackTrace = StackTraceLogType.ScriptOnly};

        public LogTypeSettings Info {
            get { return _info; }
        }

        public LogTypeSettings Debug {
            get { return _debug; }
        }

        public LogTypeSettings Warning {
            get { return _warning; }
        }

        public LogTypeSettings Error {
            get { return _error; }
        }

        public LogTypeSettings GetTypeSettings(LogLevel level) {
            switch (level) {
                case LogLevel.Debug:
                    return Debug;
                case LogLevel.Info:
                    return Info;
                case LogLevel.Warning:
                    return Warning;
                case LogLevel.Error:
                    return Error;
                default:
                    throw new ArgumentOutOfRangeException("level", level, null);
            }
        }

        public string TimeFormat {
            get { return _timeFormat; }
            set { _timeFormat = value; }
        }

    }

    public enum LogLevel {

        Debug = 0,
        Info,
        Warning,
        Error

    }

    public interface ILog {

        void Log(LogLevel logType, string format, params object[] objs);

    }

    public static class LoggerExtensions {

        public static void Info(this ILog log, string format, params object[] objs) { log.Log(LogLevel.Info, format, objs); }
        public static void Debug(this ILog log, string format, params object[] objs) { log.Log(LogLevel.Debug, format, objs); }
        public static void Warning(this ILog log, string format, params object[] objs) { log.Log(LogLevel.Warning, format, objs); }
        public static void Error(this ILog log, string format, params object[] objs) { log.Log(LogLevel.Error, format, objs); }

    }

    public static class Log {

        static Log() {
            //Task.UnhandledException += (src, args) => {
            //    Error(args.ExceptionObject);
            //};
        }

        public static event Action<string> OnLog;

        static LogSettings _settings = new LogSettings();
#if UNITY_EDITOR
        static readonly Dictionary<LogLevel, string> _colors = new Dictionary<LogLevel, string> {
            { LogLevel.Info, "green" },
            { LogLevel.Debug, "blue" },
            { LogLevel.Warning, "yellow" },
            { LogLevel.Error, "red" },
        };
#endif

        public static LogSettings Settings {
            get { return _settings; }
            set { _settings = Argument.NotNull(value); }
        }

        class Logger : ILog {

            string Prefix { get; set; }

            public Logger(string prefix) { Prefix = prefix; }

            void ILog.Log(LogLevel logType, string format, params object[] objs) {
                WriteLog(logType, "[{0}] ".With(Prefix) + format, objs);
            }

        }

        public static ILog GetLogger(string prefix) {
            return new Logger(prefix);
        }

        public static ILog GetLogger(object obj) { return GetLogger(obj.GetType()); }
        public static ILog GetLogger(Type type) { return GetLogger(type.Name); }
        public static ILog GetLogger<T>() { return GetLogger(typeof(T)); }

        public static void Info(object source, params object[] objs) {
            WriteLog(LogLevel.Info, source, objs);
        }

        public static void Debug(object source, params object[] objs) {
            WriteLog(LogLevel.Debug, source, objs);
        }

        public static void Warning(object source, params object[] objs) {
            WriteLog(LogLevel.Warning, source, objs);
        }

        public static void Error(object source, params object[] objs) {
            WriteLog(LogLevel.Error, source, objs);
        }

        static void WriteLog(LogLevel log, object source, params object[] objs) {
            var settings = Settings.GetTypeSettings(log);
            if (!Settings.GetTypeSettings(log).Enabled)
                return;
            var date = DateTime.Now.ToString(_settings.TimeFormat);
            var type = log.ToString().Substring(0, 1);
#if UNITY_EDITOR
            string prefix = "<color={2}>{1}</color> {0}: ".With(date, type, _colors[log]);
#else
            string prefix = "{1} {0}:".With(date, type);
#endif
            var output = source as string;
            if (output != null)
                output = output.With(objs);
            else
                output = source == null ? "Null" : source.ToString();
#if UNITY_EDITOR
            var level = log == LogLevel.Error ? LogType.Error : LogType.Log;
            StackTraceLogType logType = Application.GetStackTraceLogType(level);
            Application.SetStackTraceLogType(level, settings.StackTrace);
            UnityEngine.Debug.unityLogger.LogFormat(level, prefix + output, objs);
            Application.SetStackTraceLogType(level, logType);
#else
            System.Console.WriteLine(prefix + output, objs);
            if (OnLog != null)
                OnLog(string.Format(prefix + output, objs));
#endif
        }

    }

}
