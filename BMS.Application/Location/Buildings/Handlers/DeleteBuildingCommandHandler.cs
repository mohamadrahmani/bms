using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Buildings.Commands;
using BMS.Application.Models;
using BMS.Domain.Entities.BMS;
using MediatR;

public sealed class DeleteBuildingCommandHandler
    : IRequestHandler<DeleteBuildingCommand, ApiResponse<bool>>
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

    public async Task<ApiResponse<bool>> Handle(
        DeleteBuildingCommand request,
        CancellationToken cancellationToken)
    {
        var building = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (building == null)
            //throw new Exception("Building not found");
            throw new Exception("ساختمان مورد نظر پیدا نشد.");


        if (building.SiteId != request.SiteId)
            //throw new Exception("Building does not belong to this site");
            throw new Exception("این ساختمان متعلق به سایت انتخاب‌شده نیست.");


        _repository.Delete(building);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true, "ساختمان حذف شد"); ;
    }
}
