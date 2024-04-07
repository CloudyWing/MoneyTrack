using System;
using NLog;

namespace CloudyWing.MoneyTrack.Infrastructure.Logging {
    public class NLogLogger : Logger {
        private readonly NLog.Logger logger;

        public NLogLogger(Type type) {
            logger = LogManager.GetLogger(type.FullName);
        }

        public override void Log(LogLevel logLevel, Exception exception, string message, params object[] args) {
            NLog.LogLevel nLogLogLevel = ConvertLogLevel(logLevel);
            if (!logger.IsEnabled(nLogLogLevel)) {
                return;
            }

            LogEventInfo info = new LogEventInfo(nLogLogLevel, logger.Name, null, message, args, exception);
            logger.Log(typeof(Logger), info);
        }

        private static NLog.LogLevel ConvertLogLevel(LogLevel logLevel) {
            switch (logLevel) {
                case LogLevel.Trace:
                    return NLog.LogLevel.Trace;
                case LogLevel.Debug:
                    return NLog.LogLevel.Debug;
                case LogLevel.Information:
                    return NLog.LogLevel.Info;
                case LogLevel.Warning:
                    return NLog.LogLevel.Warn;
                case LogLevel.Error:
                    return NLog.LogLevel.Error;
                case LogLevel.Critical:
                    return NLog.LogLevel.Fatal;
                case LogLevel.None:
                    return NLog.LogLevel.Off;
                default:
                    return NLog.LogLevel.Debug;
            }
        }
    }
}
