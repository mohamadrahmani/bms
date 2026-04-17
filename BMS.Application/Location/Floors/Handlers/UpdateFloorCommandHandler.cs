using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Floors.Commands;
using MediatR;

public class UpdateFloorCommandHandler : IRequestHandler<UpdateFloorCommand>
{
    private readonly IFloorRepository _repository;
    private readonly IBuildingRepository _buildingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFloorCommandHandler(
        IFloorRepository repository,
        IBuildingRepository buildingRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _buildingRepository = buildingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        UpdateFloorCommand request,
        CancellationToken cancellationToken)
    {
        var floor = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (floor == null)
            throw new Exception("طبقه پیدا نشد..");
        var buildingExists = await _buildingRepository.ExistsAsync(request.BuildingId);

        if (!buildingExists)
            throw new Exception("Building not found");
        // Track Changes
        TrackChange(request, "BuildingId", floor.BuildingId.ToString(), request.BuildingId.ToString());
        TrackChange(request, "Name", floor.Name, request.Name);
        TrackChange(request, "LevelNumber", floor.LevelNumber.ToString(), request.LevelNumber.ToString());
        TrackChange(request, "Description", floor.Description, request.Description);

        floor.Update(
            request.BuildingId,
            request.Name,
            request.LevelNumber,
            request.Description);

        _repository.Update(floor);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
    private static void TrackChange(UpdateFloorCommand request, string field, string? oldValue, string? newValue)
    {
        if (oldValue != newValue)
        {
            request.Changes.Add((field, oldValue, newValue));
        }
    }
}
