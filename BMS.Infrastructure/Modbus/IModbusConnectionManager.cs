using BMS.Application.Enum;
using NModbus;


namespace BMS.Infrastructure.Modbus;

public interface IModbusConnectionManager : IDisposable
{
    Task<IModbusMaster> GetMasterAsync(CancellationToken cancellationToken);

    Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action);

    ConnectionState State { get; }

    DateTime? LastSuccessfulRead { get; }

    int ConsecutiveFailures { get; }
}

