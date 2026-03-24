using MediatR;

namespace BMS.Application.CommandDefinitions.Commands;

public class DeleteCommandDefinitionCommand : IRequest
{
    public Guid Id { get; set; }

    public DeleteCommandDefinitionCommand(Guid id)
    {
        Id = id;
    }
}
