using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Floors.Commands;
using BMS.Application.Models;
using BMS.Domain.Entities.BMS;
using MediatR;

public class UpdateFloorCommandHandler : IRequestHandler<UpdateFloorCommand, ApiResponse<bool>>
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

    public async Task<ApiResponse<bool>> Handle(
        UpdateFloorCommand request,
        CancellationToken cancellationToken)
    {
        var floor = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (floor == null)
            throw new Exception("Floor not found");
        var buildingExists = await _buildingRepository.ExistsAsync(request.BuildingId);

        if (!buildingExists)
            throw new Exception("Building not found");

        floor.Update(
            request.BuildingId,
            request.Name,
            request.LevelNumber,
            request.Description);

        _repository.Update(floor);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true, "طبقه به روزرسانی شد");
    }
}
