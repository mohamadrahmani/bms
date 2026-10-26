using System.Globalization;
using Microsoft.Extensions.Logging;

namespace BMS.Worker.Logging;

/// <summary>
/// Writes one structured, single-line record per log event to a single UTF-8 log file.
/// </summary>
public sealed class WorkerFileLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly StreamWriter _writer;
    private readonly object _writeLock = new();
    private IExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();
    private bool _disposed;

    public WorkerFileLoggerProvider(string filePath)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        var stream = new FileStream(
            filePath,
            FileMode.Append,
            FileAccess.Write,
            FileShare.ReadWrite);

        _writer = new StreamWriter(stream, new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
        {
            AutoFlush = true
        };
    }

    public ILogger CreateLogger(string categoryName) =>
        new WorkerFileLogger(categoryName, Write, () => _scopeProvider);

    public void SetScopeProvider(IExternalScopeProvider scopeProvider) =>
        _scopeProvider = scopeProvider;

    private void Write(
        LogLevel level,
        string category,
        EventId eventId,
        string message,
        Exception? exception,
        IExternalScopeProvider scopeProvider)
    {
        if (_disposed)
            return;

        var timestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture);
        var scopes = new List<string>();
        scopeProvider.ForEachScope((scope, state) => state.Add(scope?.ToString() ?? string.Empty), scopes);
        var scopeText = scopes.Count == 0 ? string.Empty : $" | Scopes={string.Join(" => ", scopes)}";
        var exceptionText = exception is null ? string.Empty : $" | Exception={Flatten(exception.ToString())}";
        var eventText = eventId.Id == 0 ? string.Empty : $" | EventId={eventId.Id}";

        var line = $"{timestamp} | {level,-11} | {category}{eventText}{scopeText} | {Flatten(message)}{exceptionText}";

        lock (_writeLock)
        {
            if (_disposed)
                return;

            _writer.WriteLine(line);
        }
    }

    private static string Flatten(string value) =>
        value.Replace("\r", " ").Replace("\n", " ").Trim();

    public void Dispose()
    {
        lock (_writeLock)
        {
            if (_disposed)
                return;

            _disposed = true;
            _writer.Dispose();
        }
    }

    private sealed class WorkerFileLogger : ILogger
    {
        private readonly string _category;
        private readonly Action<LogLevel, string, EventId, string, Exception?, IExternalScopeProvider> _write;
        private readonly Func<IExternalScopeProvider> _getScopeProvider;

        public WorkerFileLogger(
            string category,
            Action<LogLevel, string, EventId, string, Exception?, IExternalScopeProvider> write,
            Func<IExternalScopeProvider> getScopeProvider)
        {
            _category = category;
            _write = write;
            _getScopeProvider = getScopeProvider;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull =>
            _getScopeProvider().Push(state);

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            _write(logLevel, _category, eventId, formatter(state, exception), exception, _getScopeProvider());
        }
    }
}
