using System;
using System.Collections.Generic;
using OROptimizer.Diagnostics.Log;

namespace TestsSharedLibrary.Diagnostics.Log
{
    public class Log4Tests : ILog
    {
        #region Member Variables

        private static readonly List<Exception> _loggedExceptions = new List<Exception>();

        private static readonly Dictionary<LogLevel, int> _logLevelToLogsCount = new Dictionary<LogLevel, int>();

        #endregion

        #region  Constructors

        static Log4Tests()
        {
            LogLevel = LogLevel.Warn;
        }

        #endregion

        #region ILog Interface Implementation

        public void Debug(string message)
        {
            LogMessage(LogLevel.Debug, message);
        }

        public void Debug(string message, Exception exception)
        {
            LogException(LogLevel.Debug, message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            LogMessage(LogLevel.Debug, format, args);
        }

        public void Error(string message)
        {
            LogMessage(LogLevel.Error, message);
        }

        public void Error(Exception exception)
        {
            LogException(LogLevel.Error, string.Empty, exception);
        }

        public void Error(string message, Exception exception)
        {
            LogException(LogLevel.Error, message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            LogMessage(LogLevel.Error, format, args);
        }

        public void Fatal(string message)
        {
            LogMessage(LogLevel.Fatal, message);
        }

        public void Fatal(Exception exception)
        {
            LogException(LogLevel.Fatal, string.Empty, exception);
        }

        public void Fatal(string message, Exception exception)
        {
            LogException(LogLevel.Fatal, message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            LogMessage(LogLevel.Fatal, format, args);
        }

        public void Info(string message, Exception exception)
        {
            LogException(LogLevel.Info, message, exception);
        }

        public void Info(string message)
        {
            LogMessage(LogLevel.Info, message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            LogMessage(LogLevel.Info, format, args);
        }

        public bool IsDebugEnabled => ShouldLog(LogLevel.Debug);

        public bool IsErrorEnabled => ShouldLog(LogLevel.Error);

        public bool IsFatalEnabled => ShouldLog(LogLevel.Fatal);

        public bool IsInfoEnabled => ShouldLog(LogLevel.Info);

        public bool IsWarnEnabled => ShouldLog(LogLevel.Warn);

        public void Warn(string message)
        {
            LogMessage(LogLevel.Warn, message);
        }

        public void Warn(string message, Exception exception)
        {
            LogException(LogLevel.Warn, message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            LogMessage(LogLevel.Warn, format, args);
        }

        #endregion

        #region Member Functions

        public static int GetLogsCount(LogLevel level)
        {
            return _logLevelToLogsCount.TryGetValue(level, out var count) ? count : 0;
        }

        public static int GetLogsCountAtLevelOrHigher(LogLevel level)
        {
            var count = 0;
            foreach (var currLevel in Enum.GetValues(typeof(LogLevel)))
            {
                var currLogLevel = (LogLevel) currLevel;

                if (currLogLevel >= level)
                    count += GetLogsCount(currLogLevel);
            }

            return count;
        }

        private static void IncrementLevelCount(LogLevel level)
        {
            if (_logLevelToLogsCount.ContainsKey(level))
                ++_logLevelToLogsCount[level];
            else
                _logLevelToLogsCount[level] = 1;
        }

        private void LogException(LogLevel level, string message, Exception exception)
        {
            IncrementLevelCount(level);

            if (!ShouldLog(level))
                return;

            Console.Out.WriteLine($"{level}: {message}{Environment.NewLine}Exception:{exception.Message}{Environment.NewLine}{exception.StackTrace}");
            _loggedExceptions.Add(exception);
        }

        public static IReadOnlyList<Exception> LoggedExceptions => _loggedExceptions;

        public static LogLevel LogLevel { get; set; }

        private void LogMessage(LogLevel level, string format, params object[] args)
        {
            IncrementLevelCount(level);

            if (!ShouldLog(level))
                return;

            var message = args == null || args.Length == 0 ? format : string.Format(format, args);

            Console.Out.WriteLine($"{level}: {message}");
        }

        public static void ResetLogStatistics()
        {
            _logLevelToLogsCount.Clear();
            _loggedExceptions.Clear();
        }

        private bool ShouldLog(LogLevel level)
        {
            return level >= LogLevel;
        }

        #endregion
    }
}