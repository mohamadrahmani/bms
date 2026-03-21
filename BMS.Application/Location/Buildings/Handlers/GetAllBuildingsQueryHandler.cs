using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Buildings.Dtos;
using BMS.Application.Location.Buildings.Queries;

public sealed class GetAllBuildingsQueryHandler
    : IRequestHandler<GetAllBuildingsQuery, List<BuildingDto>>
{
    private readonly IBuildingRepository _repository;

    public GetAllBuildingsQueryHandler(IBuildingRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BuildingDto>> Handle(
        GetAllBuildingsQuery request,
        CancellationToken cancellationToken)
    {
        var buildings = await _repository.GetAllAsync(cancellationToken);

        return buildings.Select(x => new BuildingDto
        {
            Id = x.Id,
            SiteId = x.SiteId,
            Name = x.Name,
            Code = x.Code,
            Description = x.Description
        }).ToList();
    }
}
