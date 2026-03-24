using BMS.Application.Common.Interfaces;
using BMS.Application.Devices.DTOs;
using BMS.Application.Models;
using BMS.Application.CommandDefinitions.Commands;
using BMS.Application.CommandDefinitions.Dtos;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Location;
using MediatR;

namespace BMS.Application.CommandDefinitions.Handlers;

public class UpdateCommandDefinitionCommandHandler : IRequestHandler<UpdateCommandDefinitionCommand, ApiResponse<CommandDefinitionDto>>
{
    private readonly ICommandDefinitionRepository _repository;

    public UpdateCommandDefinitionCommandHandler(ICommandDefinitionRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<CommandDefinitionDto>> Handle(UpdateCommandDefinitionCommand request, CancellationToken cancellationToken)
    {
        var commandDefinition = await _repository.GetByIdAsync(request.Id);

        if (commandDefinition == null)
            throw new Exception("CommandDefinition not found");


        commandDefinition.Name = request.Name;
        commandDefinition.Code = request.Code;
        commandDefinition.DeviceType = request.DeviceType;
        commandDefinition.ParameterType = request.ParameterType;
        commandDefinition.HasParameter = request.HasParameter;


        //if (request.RegisterType.HasValue && request.RegisterAddress.HasValue)
        //{
        //    CommandDefinition.SetMapping(
        //        request.RegisterType.Value,
        //        (int)request.RegisterAddress.Value,
        //        request.BitIndex,
        //        request.ByteOrder
        //    );
        //}
        //else
        //{
        //    CommandDefinition.ClearMapping();
        //}

        await _repository.UpdateAsync(commandDefinition);

        var dto = new CommandDefinitionDto
        {
            Id = commandDefinition.Id,
        };

        return ApiResponse<CommandDefinitionDto>.SuccessResponse(dto, "Device updated successfully");
    }
}
