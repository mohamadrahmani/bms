using BMS.Application.Location.Floors.Dtos;
using BMS.Application.Location.Floors.Queries;
using MediatR;

public class GetFloorsByBuildingQueryHandler
    : IRequestHandler<GetFloorsByBuildingQuery, List<FloorDto>>
{
    private readonly IFloorRepository _repository;

    public GetFloorsByBuildingQueryHandler(IFloorRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<FloorDto>> Handle(
        GetFloorsByBuildingQuery request,
        CancellationToken cancellationToken)
    {
        var floors = await _repository.GetByBuildingIdAsync(
            request.BuildingId,
            cancellationToken);

        return floors.Select(x => new FloorDto
        {
            Id = x.Id,
            BuildingId = x.BuildingId,
            Name = x.Name,
            LevelNumber = x.LevelNumber,
            Description = x.Description
        }).ToList();
    }
}
