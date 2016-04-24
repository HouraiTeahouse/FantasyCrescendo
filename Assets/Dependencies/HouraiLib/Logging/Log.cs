using System;
using UnityEngine;

namespace HouraiTeahouse {

    public static class Log {

        private delegate void LogAction(string source, params object[] param);

        public static string TimeFormat = "MM-dd-yy HH:mm:ss";

        public static bool ShowInfo = true;
        public static bool ShowDebug = true;
        public static bool ShowWarning = true;
        public static bool ShowError = true;

        public static StackTraceLogType StackTraceInfo = StackTraceLogType.None;
        public static StackTraceLogType StackTraceDebug = StackTraceLogType.None;
        public static StackTraceLogType StackTraceWarning = StackTraceLogType.None;
        public static StackTraceLogType StackTraceError = StackTraceLogType.None;

        public static void Info(string source, params object[] objs) {
#if UNITY_EDITOR
            const string format = "<color=green>[Info]</color> ({1}) - {0}";
#else
            const string format = "[Info] ({1}) - {0}";
#endif
            WriteLog(format, ShowInfo, StackTraceInfo, UnityEngine.Debug.LogFormat, source, objs);
        }

        public static void Debug(string source, params object[] objs) {
#if UNITY_EDITOR
            const string format = "<color=blue>[Debug]</color> ({1}) - {0}";
#else
            const string format = "[Debug] ({1}) - {0}";
#endif
            WriteLog(format, ShowDebug, StackTraceDebug, UnityEngine.Debug.LogFormat, source, objs);
        }

        public static void Warning(string source, params object[] objs) {
#if UNITY_EDITOR
            const string format = "<color=yellow>[Warning]</color> ({1}) -  {0}";
#else
            const string format = "[Warning] ({1}) - {0}";
#endif
            WriteLog(format, ShowWarning, StackTraceWarning, UnityEngine.Debug.LogWarningFormat, source, objs);
        }

        public static void Error(string source, params object[] objs) {
#if UNITY_EDITOR
            const string format = "<color=red>[Error]</color> ({1}) - {0}";
#else
            const string format = "[Error] {0}";
#endif
            WriteLog(format, ShowError, StackTraceError, UnityEngine.Debug.LogErrorFormat, source, objs);
        }

        private static void WriteLog(string format, bool show, StackTraceLogType stack, LogAction log, string source, params object[] objs) {
            if (!show)
                return;
            StackTraceLogType logType = Application.stackTraceLogType;
            Application.stackTraceLogType = stack;
            string output = string.Format(source, objs);
#if UNITY_EDITOR
            log(format, output, DateTime.Now.ToString(TimeFormat)); 
#else
            System.Console.WriteLine(format, output, DateTime.Now.ToString(TimeFormat));
#endif
            Application.stackTraceLogType = logType;
        }
    }
}
