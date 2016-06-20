// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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
        public LogTypeSettings Debug = new LogTypeSettings {
            Enabled = true,
            StackTrace = StackTraceLogType.None
        };

        public LogTypeSettings Error = new LogTypeSettings {
            Enabled = true,
            StackTrace = StackTraceLogType.None
        };

        public LogTypeSettings Info = new LogTypeSettings {
            Enabled = true,
            StackTrace = StackTraceLogType.None
        };

        public string TimeFormat = "MM-dd-yy HH:mm:ss";

        public LogTypeSettings Warning = new LogTypeSettings {
            Enabled = true,
            StackTrace = StackTraceLogType.None
        };
    }

    public static class Log {
        public static LogSettings _settings = new LogSettings();

        public static LogSettings Settings {
            get { return _settings; }
            set { _settings = value; }
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

        static void WriteLog(string format,
                             LogTypeSettings settings,
                             LogType log,
                             object source,
                             params object[] objs) {
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
            UnityEngine.Debug.logger.LogFormat(log,
                format,
                output,
                DateTime.Now.ToString(_settings.TimeFormat));
#else
            System.Console.WriteLine(format, output, DateTime.Now.ToString(_settings.TimeFormat));
#endif
            Application.SetStackTraceLogType(log, logType);
        }
    }
}