using BMS.Application.Common.Interfaces;
using BMS.Application.Points.Commands;
using MediatR;

namespace BMS.Application.Points.Handlers;

public class DeletePointCommandHandler : IRequestHandler<DeletePointCommand>
{
    private readonly IPointRepository _repository;

    public DeletePointCommandHandler(IPointRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeletePointCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);

        return Unit.Value;
    }
}
