using NModbus;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

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
            TimeSpan.FromSeconds(3),
            TimeoutStrategy.Pessimistic);

        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
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

    public void Dispose()
    {
        _master = null;
        _client?.Dispose();
    }
}
