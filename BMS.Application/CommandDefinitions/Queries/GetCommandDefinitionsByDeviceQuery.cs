using BMS.Application.CommandDefinitions.Dtos;
using MediatR;

namespace BMS.Application.CommandDefinitions.Queries;

public class GetCommandDefinitionsByDeviceQuery : IRequest<IEnumerable<CommandDefinitionDto>>
{
    public Guid DeviceId { get; set; }

    public GetCommandDefinitionsByDeviceQuery(Guid deviceId)
    {
        DeviceId = deviceId;
    }
}
