using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Pagination;
using BMS.Application.CommandDefinitions.Dtos;
using BMS.Application.CommandDefinitions.Queries;
using MediatR;

namespace BMS.Application.CommandDefinitions.Handlers;

public class GetCommandDefinitionsQueryHandler : IRequestHandler<GetCommandDefinitionsQuery, PagedResult<CommandDefinitionDto>>
{
    private readonly ICommandDefinitionRepository _CommandDefinitionRepository;

    public GetCommandDefinitionsQueryHandler(ICommandDefinitionRepository CommandDefinitionRepository)
    {
        _CommandDefinitionRepository = CommandDefinitionRepository;
    }

    public async Task<PagedResult<CommandDefinitionDto>> Handle(GetCommandDefinitionsQuery request, CancellationToken cancellationToken)
    {
        var CommandDefinitions = _CommandDefinitionRepository.CommandDefinitions;

        var query = CommandDefinitions.Select(p => new CommandDefinitionDto
        {
            Id = p.Id,
            ParameterType = p.ParameterType,
            Code = p.Code,
            DeviceType = p.DeviceType,
            HasParameter = p.HasParameter,
            Name = p.Name
        });
        return await query.ToPagedResultAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
