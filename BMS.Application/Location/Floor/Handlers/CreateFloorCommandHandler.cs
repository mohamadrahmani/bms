using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Floors.Commands;
using BMS.Domain.Entities.Location;
using MediatR;

public class CreateFloorCommandHandler : IRequestHandler<CreateFloorCommand, Guid>
{
    private readonly IFloorRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateFloorCommandHandler(
        IFloorRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateFloorCommand request,
        CancellationToken cancellationToken)
    {
        var floor = new Floor(
            request.BuildingId,
            request.Name,
            request.LevelNumber,
            request.Description);

        await _repository.AddAsync(floor, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return floor.Id;
    }
}
