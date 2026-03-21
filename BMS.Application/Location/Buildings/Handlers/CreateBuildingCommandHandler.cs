using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Buildings.Commands;
using BMS.Domain.Entities.Location;

public sealed class CreateBuildingCommandHandler
    : IRequestHandler<CreateBuildingCommand, Guid>
{
    private readonly IBuildingRepository _repository;
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBuildingCommandHandler(
        IBuildingRepository repository,
        ISiteRepository siteRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateBuildingCommand request,
        CancellationToken cancellationToken)
    {
        var site = await _siteRepository.GetByIdAsync(request.SiteId, cancellationToken);

        if (site == null)
            throw new Exception("Site not found");

        var codeExists = await _repository.ExistsCodeAsync(
            request.SiteId,
            request.Code,
            cancellationToken);

        if (codeExists)
            throw new Exception("Building code already exists in this site");

        var building = new Building(
            request.SiteId,
            request.Name,
            request.Code,
            request.Description
        );

        await _repository.AddAsync(building, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return building.Id;
    }
}
