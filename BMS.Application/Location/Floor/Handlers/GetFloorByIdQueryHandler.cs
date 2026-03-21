using BMS.Application.Location.Floors.Dtos;
using BMS.Application.Location.Floors.Queries;
using MediatR;

public class GetFloorByIdQueryHandler
    : IRequestHandler<GetFloorByIdQuery, FloorDto?>
{
    private readonly IFloorRepository _repository;

    public GetFloorByIdQueryHandler(IFloorRepository repository)
    {
        _repository = repository;
    }

    public async Task<FloorDto?> Handle(
        GetFloorByIdQuery request,
        CancellationToken cancellationToken)
    {
        var floor = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (floor == null)
            return null;

        return new FloorDto
        {
            Id = floor.Id,
            BuildingId = floor.BuildingId,
            Name = floor.Name,
            LevelNumber = floor.LevelNumber,
            Description = floor.Description
        };
    }
}
