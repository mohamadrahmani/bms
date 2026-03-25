using BMS.Application.Common.Exceptions;
using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Sites.Commands;
using BMS.Application.Models;
using BMS.Domain.Entities.BMS;
using MediatR;

namespace BMS.Application.Location.Sites.Handlers;

public sealed class UpdateSiteCommandHandler
    : IRequestHandler<UpdateSiteCommand, ApiResponse<bool>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSiteCommandHandler(
        ISiteRepository siteRepository,
        IUnitOfWork unitOfWork)
    {
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(
        UpdateSiteCommand request,
        CancellationToken cancellationToken)
    {
        var site = await _siteRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (site is null)
            throw new NotFoundException(
                $"Site with id '{request.Id}' not found.");

        site.Update(
            request.Name,
            request.Address,
            request.Description
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true, "اطلاعات سایت بروزرسانی شد");
    }
}
