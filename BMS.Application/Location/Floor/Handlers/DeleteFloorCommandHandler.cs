using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Floors.Commands;
using BMS.Application.Models;
using BMS.Domain.Entities.BMS;
using MediatR;

public class DeleteFloorCommandHandler : IRequestHandler<DeleteFloorCommand, ApiResponse<bool>>
{
    private readonly IFloorRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteFloorCommandHandler(
        IFloorRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(
        DeleteFloorCommand request,
        CancellationToken cancellationToken)
    {
        var floor = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (floor == null)
            throw new Exception("Floor not found");

        _repository.Remove(floor);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true, "طبقه حذف شد");
    }
}
