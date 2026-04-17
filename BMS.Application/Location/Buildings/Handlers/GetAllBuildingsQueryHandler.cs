using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Buildings.Dtos;
using BMS.Application.Location.Buildings.Queries;
using BMS.Application.Common.Pagination;
using BMS.Application.Controllers.Dtos;

public sealed class GetAllBuildingsQueryHandler
    : IRequestHandler<GetAllBuildingsQuery, PagedResult<BuildingDto>>
{
    private readonly IBuildingRepository _repository;

    public GetAllBuildingsQueryHandler(IBuildingRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<BuildingDto>> Handle(
        GetAllBuildingsQuery request,
        CancellationToken cancellationToken)
    {
        var buildings = _repository.Buildings;
        // filter by site
        if (request.SiteId.HasValue)
        {
            buildings = buildings.Where(b => b.SiteId == request.SiteId);
        }
        // search
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            buildings = buildings.Where(b =>
                b.Name.Contains(request.Search) ||
                b.Code.Contains(request.Search));
        }
        var query = buildings.Select(x => new BuildingDto
        {
            Id = x.Id,
            SiteId = x.SiteId,
            Name = x.Name,
            Code = x.Code,
            Description = x.Description
        });
        return await query.ToPagedResultAsync(
        request.PageNumber,
        request.PageSize,
        cancellationToken);

    }
}
