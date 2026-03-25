using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Buildings.Commands;
using BMS.Application.Models;
using BMS.Domain.Entities.BMS;
using MediatR;

public sealed class UpdateBuildingCommandHandler
    : IRequestHandler<UpdateBuildingCommand, ApiResponse<bool>>
{
    private readonly IBuildingRepository _repository;
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBuildingCommandHandler(
        IBuildingRepository repository,
        ISiteRepository siteRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(
        UpdateBuildingCommand request,
        CancellationToken cancellationToken)
    {
        var building = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (building == null)
            throw new Exception("Building not found");

        var siteExists = await _siteRepository.ExistsAsync(request.SiteId);

        if (!siteExists)
            throw new Exception("Site not found");
        var codeExists = await _repository.ExistsCodeAsync(request.Code, request.Id, cancellationToken);

        if (codeExists)
            throw new Exception("Building code already exists");

        building.Update(
             request.SiteId,
            request.Name,
            request.Code,
            request.Description
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true, "ساختمان بروز رسانی شد");
    }
}
