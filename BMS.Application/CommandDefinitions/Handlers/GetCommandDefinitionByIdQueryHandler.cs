using BMS.Application.Common.Interfaces;
using BMS.Application.CommandDefinitions.Dtos;
using BMS.Application.CommandDefinitions.Queries;
using MediatR;

namespace BMS.Application.CommandDefinitions.Handlers;

public class GetCommandDefinitionByIdQueryHandler : IRequestHandler<GetCommandDefinitionByIdQuery, CommandDefinitionDto?>
{
    private readonly ICommandDefinitionRepository _repository;

    public GetCommandDefinitionByIdQueryHandler(ICommandDefinitionRepository repository)
    {
        _repository = repository;
    }

    public async Task<CommandDefinitionDto?> Handle(GetCommandDefinitionByIdQuery request, CancellationToken cancellationToken)
    {
        var CommandDefinition = await _repository.GetByIdAsync(request.Id);

        if (CommandDefinition == null)
            return null;

        //return new CommandDefinitionDto
        //{
        //    Id = CommandDefinition.Id,
        //    DeviceId = CommandDefinition.DeviceId,
        //    Title = CommandDefinition.Title,
        //    Kind = CommandDefinition.Kind,
        //    DataType = CommandDefinition.DataType,
        //    Address = CommandDefinition.Address,
        //    Value = CommandDefinition.Value
        //};
        return new CommandDefinitionDto
        {
            Id = CommandDefinition.Id,
            //DeviceId = CommandDefinition.DeviceId,
            //Tag = CommandDefinition.Tag,
            //Title = CommandDefinition.Title,
            //Kind = CommandDefinition.Kind,
            //DataType = CommandDefinition.DataType,
            //Address = CommandDefinition.Address,
            //Unit = CommandDefinition.Unit,
            //Code = CommandDefinition.Code,
            //Length = CommandDefinition.Length,
            //Scale = CommandDefinition.Scale,
            //Offset = CommandDefinition.Offset,
            //IsWritable = CommandDefinition.IsWritable,
            //RegisterType = CommandDefinition.RegisterType,
            //RegisterAddress = CommandDefinition.RegisterAddress,
            //BitIndex = CommandDefinition.BitIndex,
            //ByteOrder = CommandDefinition.ByteOrder,
            //SiteId = CommandDefinition.Location?.SiteId,
            //BuildingId = CommandDefinition.Location?.BuildingId,
            //FloorId = CommandDefinition.Location?.FloorId,
            //WardId = CommandDefinition.Location?.WardId,
            //RoomId = CommandDefinition.Location?.RoomId,
            //Value = CommandDefinition.Value,
            //Quality = CommandDefinition.Quality.ToString(),
            //LastUpdatedAtUtc = CommandDefinition.LastUpdatedAtUtc
        };

    }
}
