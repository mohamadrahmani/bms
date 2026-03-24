using BMS.Application.Common.Interfaces;
using BMS.Application.CommandDefinitions.Commands;
using MediatR;

namespace BMS.Application.CommandDefinitions.Handlers;

public class DeleteCommandDefinitionCommandHandler : IRequestHandler<DeleteCommandDefinitionCommand>
{
    private readonly ICommandDefinitionRepository _repository;

    public DeleteCommandDefinitionCommandHandler(ICommandDefinitionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteCommandDefinitionCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);

        return Unit.Value;
    }
}
