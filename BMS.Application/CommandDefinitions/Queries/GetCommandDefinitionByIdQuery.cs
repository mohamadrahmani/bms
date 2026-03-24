using BMS.Application.CommandDefinitions.Dtos;
using MediatR;

namespace BMS.Application.CommandDefinitions.Queries;

public class GetCommandDefinitionByIdQuery : IRequest<CommandDefinitionDto?>
{
    public Guid Id { get; set; }

    public GetCommandDefinitionByIdQuery(Guid id)
    {
        Id = id;
    }
}
