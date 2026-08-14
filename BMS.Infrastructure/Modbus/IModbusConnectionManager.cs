using BMS.Application.Enum;
using NModbus;


namespace BMS.Infrastructure.Modbus;

public interface IModbusConnectionManager : IDisposable
{
    Task<IModbusMaster> GetMasterAsync(CancellationToken cancellationToken);

    Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action);
    Task ExecuteWithRetryAsync(Func<Task> action);
    ConnectionState State { get; }

    DateTime? LastSuccessfulRead { get; }

    int ConsecutiveFailures { get; }
    Task<T> ExecuteReadAsync<T>(Func<Task<T>> action);
    Task<bool> PingAsync(CancellationToken cancellationToken);
    Task<IModbusMaster> GetMasterForReadAsync(CancellationToken cancellationToken);
}

