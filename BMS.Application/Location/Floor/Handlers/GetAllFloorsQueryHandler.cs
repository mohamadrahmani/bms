using BMS.Application.Location.Floors.Dtos;
using BMS.Application.Location.Floors.Queries;
using MediatR;

public class GetAllFloorsQueryHandler
    : IRequestHandler<GetAllFloorsQuery, List<FloorDto>>
{
    private readonly IFloorRepository _repository;

    public GetAllFloorsQueryHandler(IFloorRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<FloorDto>> Handle(
        GetAllFloorsQuery request,
        CancellationToken cancellationToken)
    {
        var floors = await _repository.GetAllAsync(cancellationToken);

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
