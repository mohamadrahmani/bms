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
            //throw new NotFoundException($"Site with id '{request.Id}' not found.");
            throw new NotFoundException(($"سایت '{request.Id}'مورد نظر پیدا نشد."));

        // Track Changes
        void Track(string field, string? oldValue, string? newValue)
        {
            if (oldValue != newValue)
                request.Changes.Add((field, oldValue, newValue));
        }

        Track("Name", site.Name, request.Name);
        Track("Address", site.Address, request.Address);
        Track("Description", site.Description, request.Description);


        site.Update(
            request.Name,
            request.Address,
            request.Description
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true, "اطلاعات سایت بروزرسانی شد");
    }
}
