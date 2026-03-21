using MediatR;

namespace BMS.Application.Location.Floors.Commands;

public class UpdateFloorCommand : IRequest
{
    public Guid Id { get; set; }
    public Guid BuildingId { get; set; }
    public string Name { get; set; } = default!;

    public int LevelNumber { get; set; }

    public string? Description { get; set; }
}
