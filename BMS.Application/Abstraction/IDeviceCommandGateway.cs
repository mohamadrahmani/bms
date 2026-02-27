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
            string commandName,
            object? payload);
    }
}