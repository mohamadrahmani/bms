using System.Threading.Tasks;
using BMS.Domain.Entities;
using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;

namespace BMS.Application.Interfaces
{
    public interface IDeviceCommandGateway
    {
        Task<CommandResult> ExecuteAsync(
            string deviceId,
            string pointId,
            string commandName,
            string? value);

        Task<bool> SendAsync(PointWriteCommand command, CancellationToken ct);
    }

}