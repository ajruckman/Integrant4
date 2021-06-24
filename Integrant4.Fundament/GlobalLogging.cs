using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Integrant4.Fundament
{
    internal class GlobalLoggerProvider : ILoggerProvider
    {
        private readonly GlobalLogger _logger;

        public GlobalLoggerProvider(GlobalLogger logger)
        {
            _logger = logger;
        }

        public ILogger CreateLogger(string _)
        {
            return new GlobalLoggerInternal(_logger);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    internal class GlobalLoggerInternal : ILogger
    {
        private readonly GlobalLogger _globalLogger;

        internal GlobalLoggerInternal(GlobalLogger globalLogger)
        {
            _globalLogger = globalLogger;
        }

        public IDisposable? BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel == LogLevel.Error;

        public void Log<TState>
        (
            LogLevel                         logLevel,
            EventId                          eventId,
            TState                           state,
            Exception?                       exception,
            Func<TState, Exception?, string> formatter
        )
        {
            IReadOnlyDictionary<string, object>? dict = null;

            if (state is IReadOnlyList<KeyValuePair<string, object>> list)
            {
                dict = list.ToDictionary(k => k.Key, v => v.Value);
            }

            _globalLogger.Invoke(new LogEvent
            (
                logLevel,
                exception,
                formatter.Invoke(state, exception),
                dict
            ));
        }
    }

    public readonly struct LogEvent
    {
        public LogEvent
        (
            LogLevel                             logLevel,
            Exception?                           exception,
            string                               message,
            IReadOnlyDictionary<string, object>? fields
        )
        {
            LogLevel  = logLevel;
            Exception = exception;
            Message   = message;
            Fields    = fields;
        }

        public readonly LogLevel                             LogLevel;
        public readonly Exception?                           Exception;
        public readonly string                               Message;
        public readonly IReadOnlyDictionary<string, object>? Fields;
    }

    public class GlobalLogger
    {
        public event Action<LogEvent>? OnEvent;

        internal void Invoke(LogEvent logEvent)
        {
            OnEvent?.Invoke(logEvent);
        }
    }

    public static class GlobalLoggerExtensions
    {
        public static void AddGlobalLogger(this ILoggingBuilder builder, GlobalLogger logger)
        {
            builder.Services.AddSingleton<ILoggerProvider, GlobalLoggerProvider>(_ => new GlobalLoggerProvider(logger));
        }
    }
}