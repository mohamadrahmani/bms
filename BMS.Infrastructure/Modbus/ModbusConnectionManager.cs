using NModbus;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using BMS.Application.Enum;

namespace BMS.Infrastructure.Modbus;

public class ModbusConnectionManager : IModbusConnectionManager
{
    private readonly ILogger<ModbusConnectionManager> _logger;
    private readonly string _ip;
    private readonly int _port;
    private TcpClient? _client;
    private IModbusMaster? _master;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly IAsyncPolicy _policyWrap;

    private readonly object _lock = new();
    private ConnectionState _state = ConnectionState.Unknown;
    private DateTime? _lastSuccessfulRead;
    private int _consecutiveFailures;

    public ConnectionState State => _state;
    public DateTime? LastSuccessfulRead => _lastSuccessfulRead;
    public int ConsecutiveFailures => _consecutiveFailures;
    private readonly IAsyncPolicy _readPolicy;

    // در سازنده، بعد از تعریف circuitBreakerPolicy و timeoutPolicy اصلی:

    public ModbusConnectionManager(string ip, int port, ILogger<ModbusConnectionManager> logger)
    {
        _ip = ip;
        _port = port;
        _logger = logger;
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: _ => TimeSpan.FromSeconds(1),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        exception,
                        "Retry {RetryCount} after {Delay}s",
                        retryCount,
                        timeSpan.TotalSeconds);
                });

        var timeoutPolicy = Policy.TimeoutAsync(
            TimeSpan.FromMilliseconds(500),
            TimeoutStrategy.Pessimistic);

        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(10),
                onBreak: (exception, breakDelay) =>
                {
                    _state = ConnectionState.Offline;
                    _logger.LogError(
                        exception,
                        "Circuit opened for {Delay}s",
                        breakDelay.TotalSeconds);
                },
                onReset: () =>
                {
                    _state = ConnectionState.Online;
                    _consecutiveFailures = 0;
                    _logger.LogInformation("Circuit closed. Connection restored.");
                },
                onHalfOpen: () =>
                {
                    _state = ConnectionState.HalfOpen;
                    _logger.LogWarning("Circuit half-open. Testing connection...");
                });

        _policyWrap = Policy.WrapAsync(
            circuitBreakerPolicy,
            retryPolicy,
            timeoutPolicy
        );
        _readPolicy = Policy.WrapAsync(circuitBreakerPolicy, timeoutPolicy);

    }

    public async Task<T> ExecuteReadAsync<T>(Func<Task<T>> action)
    {
        try
        {
            var result = await _readPolicy.ExecuteAsync(action);
            _state = ConnectionState.Online;
            _lastSuccessfulRead = DateTime.UtcNow;
            _consecutiveFailures = 0;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Read execution failed.");
            _consecutiveFailures++;
            throw;
        }
    }

    // overload برای Action بدون بازگشت (void)
    public async Task ExecuteReadAsync(Func<Task> action)
    {
        await ExecuteReadAsync<object>(async () =>
        {
            await action().ConfigureAwait(false);
            return null!;
        });
    }
    public async Task<IModbusMaster> GetMasterAsync(CancellationToken cancellationToken)
    {
        if (_master != null && _client?.Connected == true)
            return _master;

        lock (_lock)
        {
            if (_master != null && _client?.Connected == true)
                return _master;

            _client?.Dispose();
            _client = new TcpClient();
        }
        //برای retry
        await _policyWrap.ExecuteAsync(async () =>
        {
            await _client!.ConnectAsync(_ip, _port, cancellationToken);
        });

        var factory = new ModbusFactory();
        _master = factory.CreateMaster(_client);
        _state = ConnectionState.Online;
        _lastSuccessfulRead = DateTime.UtcNow;
        _consecutiveFailures = 0;

        return _master;
    }



    public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action)
    {
        try
        {
            var result = await _policyWrap.ExecuteAsync(action);

            _state = ConnectionState.Online;
            _lastSuccessfulRead = DateTime.UtcNow;
            _consecutiveFailures = 0;

            return result;
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "Circuit is open. Skipping execution.");

            _consecutiveFailures++;
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Execution failed.");

            _consecutiveFailures++;
            throw;
        }
    }
    //public async Task ExecuteWithRetryAsync(Func<Task> action)
    //{
    //    await _policyWrap.ExecuteAsync(async () =>
    //    {
    //        await action();
    //        return true;
    //    });
    //}
    public Task ExecuteWithRetryAsync(Func<Task> action)
    {
        if (action is null) throw new ArgumentNullException(nameof(action));

        return ExecuteWithRetryAsync<object>(async () =>
        {
            await action().ConfigureAwait(false);
            return null!;
        });
    }
    public void Dispose()
    {
        _master = null;
        _client?.Dispose();
    }
    public async Task<bool> PingAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromMilliseconds(300)); // حداکثر ۵۰۰ میلی‌ثانیه

            using var client = new TcpClient();
            await client.ConnectAsync(_ip, _port, cts.Token);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IModbusMaster> GetMasterForReadAsync(CancellationToken cancellationToken)
    {
        // اگر اتصال قبلی هنوز برقرار است، از همان استفاده کن
        if (_master != null && _client?.Connected == true)
            return _master;

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromMilliseconds(500)); // نهایت ۵۰۰ میلی‌ثانیه برای اتصال

            var client = new TcpClient();
            await client.ConnectAsync(_ip, _port, cts.Token);

            var factory = new ModbusFactory();
            var newMaster = factory.CreateMaster(client);

            // ذخیره کن تا بعداً دوباره استفاده شود
            lock (_lock)
            {
                _client?.Dispose();
                _client = client;
                _master = newMaster;
                _state = ConnectionState.Online;
                _lastSuccessfulRead = DateTime.UtcNow;
                _consecutiveFailures = 0;
            }
            return _master;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fast master connection failed for read.");
            throw;
        }
    }
}
