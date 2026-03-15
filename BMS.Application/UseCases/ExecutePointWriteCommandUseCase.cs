using BMS.Application.Abstraction;
using BMS.Application.Interfaces;
using BMS.Application.Models;

namespace BMS.Application.UseCases;

public class ExecutePointWriteCommandUseCase
{
    private readonly IDeviceCommandGateway _dispatcher;

    public ExecutePointWriteCommandUseCase(IDeviceCommandGateway dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public async Task<bool> ExecuteAsync(PointWriteCommand command, CancellationToken ct)
    {
        return await _dispatcher.SendAsync(command, ct);
    }
}
