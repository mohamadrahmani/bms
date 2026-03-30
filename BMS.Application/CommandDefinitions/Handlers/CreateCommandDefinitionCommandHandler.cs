using BMS.Application.CommandDefinitions.Commands;
using BMS.Application.CommandDefinitions.Dtos;
using BMS.Application.Common.Interfaces;
using BMS.Application.Models;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Location;
using MediatR;

namespace BMS.Application.CommandDefinitions.Handlers;

public class CreateCommandDefinitionCommandHandler : IRequestHandler<CreateCommandDefinitionCommand, ApiResponse<Guid>>
{
    private readonly ICommandDefinitionRepository _repository;

    public CreateCommandDefinitionCommandHandler(ICommandDefinitionRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<Guid>> Handle(CreateCommandDefinitionCommand request, CancellationToken cancellationToken)
    {
        var commandDefinition = new CommandDefinition() { 
            Code = request.Code,
            Name = request.Name,
            CommandType = request.CommandType,
            DeviceType = request.DeviceType,
            OnState = request.OnState,
            OffState = request.OffState,
            HasParameter = request.HasParameter,
            ParameterType = request.ParameterType
        };

        await _repository.AddAsync(commandDefinition);
        

        return ApiResponse<Guid>.SuccessResponse(commandDefinition.Id, "Device created successfully");

    }
}
