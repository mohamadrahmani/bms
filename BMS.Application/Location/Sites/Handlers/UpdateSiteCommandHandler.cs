using MediatR;
using BMS.Application.Location.Sites.Commands;
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Exceptions;

namespace BMS.Application.Location.Sites.Handlers;

public sealed class UpdateSiteCommandHandler
    : IRequestHandler<UpdateSiteCommand>
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

    public async Task<Unit> Handle(
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
        return Unit.Value;
    }
}
