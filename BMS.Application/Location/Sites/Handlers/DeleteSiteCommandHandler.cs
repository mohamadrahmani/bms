using BMS.Application.Common.Exceptions;
using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Sites.Commands;
using BMS.Application.Models;
using BMS.Domain.Entities.BMS;
using MediatR;

namespace BMS.Application.Location.Sites.Handlers;

public sealed class DeleteSiteCommandHandler
    : IRequestHandler<DeleteSiteCommand, ApiResponse<bool>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSiteCommandHandler(
        ISiteRepository siteRepository,
        IUnitOfWork unitOfWork)
    {
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(
        DeleteSiteCommand request,
        CancellationToken cancellationToken)
    {
        var site = await _siteRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (site is null)
            throw new NotFoundException(
                $"Site with id '{request.Id}' not found.");

        _siteRepository.Delete(site);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true, "سایت حذف شد");
    }
}
