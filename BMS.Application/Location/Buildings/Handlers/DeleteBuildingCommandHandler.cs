using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Buildings.Commands;

public sealed class DeleteBuildingCommandHandler
    : IRequestHandler<DeleteBuildingCommand>
{
    private readonly IBuildingRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBuildingCommandHandler(
        IBuildingRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        DeleteBuildingCommand request,
        CancellationToken cancellationToken)
    {
        var building = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (building == null)
            throw new Exception("Building not found");

        _repository.Delete(building);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
