using System;

namespace CloudyWing.MoneyTrack.Infrastructure.Logging {
    public abstract class Logger {
        public abstract void Log(LogLevel logLevel, Exception exception, string message, params object[] args);

        public void Log(LogLevel logLevel, string message, params object[] args) {
            Log(logLevel, null, message, args);
        }

        public void Log(LogLevel logLevel, Exception exception) {
            Log(logLevel, exception, null);
        }

        public void LogTrace(string message, params object[] args) {
            Log(LogLevel.Trace, message, args);
        }

        public void LogTrace(Exception exception) {
            Log(LogLevel.Trace, exception);
        }

        public void LogTrace(Exception exception, string message, params object[] args) {
            Log(LogLevel.Trace, exception, message, args);
        }

        public void LogDebug(string message, params object[] args) {
            Log(LogLevel.Debug, message, args);
        }

        public void LogDebug(Exception exception) {
            Log(LogLevel.Debug, exception);
        }

        public void LogDebug(Exception exception, string message, params object[] args) {
            Log(LogLevel.Debug, exception, message, args);
        }

        public void LogInformation(string message, params object[] args) {
            Log(LogLevel.Information, message, args);
        }

        public void LogInformation(Exception exception) {
            Log(LogLevel.Information, exception);
        }

        public void LogInformation(Exception exception, string message, params object[] args) {
            Log(LogLevel.Information, exception, message, args);
        }

        public void LogWarning(string message, params object[] args) {
            Log(LogLevel.Warning, message, args);
        }

        public void LogWarning(Exception exception) {
            Log(LogLevel.Warning, exception);
        }

        public void LogWarning(Exception exception, string message, params object[] args) {
            Log(LogLevel.Warning, exception, message, args);
        }

        public void LogError(string message, params object[] args) {
            Log(LogLevel.Error, message, args);
        }

        public void LogError(Exception exception) {
            Log(LogLevel.Error, exception);
        }

        public void LogError(Exception exception, string message, params object[] args) {
            Log(LogLevel.Error, exception, message, args);
        }

        public void LogCritical(string message, params object[] args) {
            Log(LogLevel.Critical, message, args);
        }

        public void LogCritical(Exception exception) {
            Log(LogLevel.Critical, exception);
        }

        public void LogCritical(Exception exception, string message, params object[] args) {
            Log(LogLevel.Critical, exception, message, args);
        }
    }
}
