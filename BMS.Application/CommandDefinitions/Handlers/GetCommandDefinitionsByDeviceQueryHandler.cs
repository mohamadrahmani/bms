using BMS.Application.Common.Interfaces;
using BMS.Application.CommandDefinitions.Dtos;
using BMS.Application.CommandDefinitions.Queries;
using MediatR;

namespace BMS.Application.CommandDefinitions.Handlers;

public class GetCommandDefinitionsByDeviceQueryHandler : IRequestHandler<GetCommandDefinitionsByDeviceQuery, IEnumerable<CommandDefinitionDto>>
{
    private readonly ICommandDefinitionRepository _repository;

    public GetCommandDefinitionsByDeviceQueryHandler(ICommandDefinitionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CommandDefinitionDto>> Handle(GetCommandDefinitionsByDeviceQuery request, CancellationToken cancellationToken)
    {
        var CommandDefinitions = await _repository.GetAllAsync(cancellationToken);

        //return CommandDefinitions.Select(p => new CommandDefinitionDto
        //{
        //    Id = p.Id,
        //    DeviceId = p.DeviceId,
        //    Title = p.Title,
        //    Kind = p.Kind,
        //    DataType = p.DataType,
        //    Address = p.Address,
        //    Value = p.Value
        //});
        return CommandDefinitions.Select(p => new CommandDefinitionDto
        {
            Id = p.Id,
            //DeviceId = p.DeviceId,
            //Tag = p.Tag,
            //Title = p.Title,
            //Kind = p.Kind,
            //DataType = p.DataType,
            //Address = p.Address,
            //Unit = p.Unit,
            //Code = p.Code,
            //Length = p.Length,
            //Scale = p.Scale,
            //Offset = p.Offset,
            //IsWritable = p.IsWritable,
            //RegisterType = p.RegisterType,
            //RegisterAddress = p.RegisterAddress,
            //BitIndex = p.BitIndex,
            //ByteOrder = p.ByteOrder,
            //SiteId = p.Location?.SiteId,
            //BuildingId = p.Location?.BuildingId,
            //FloorId = p.Location?.FloorId,
            //WardId = p.Location?.WardId,
            //RoomId = p.Location?.RoomId,
            //Value = p.Value,
            //Quality = p.Quality.ToString(),
            //LastUpdatedAtUtc = p.LastUpdatedAtUtc
        });

    }
}
